using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using StocksApi.Core.Configuration;
using StocksApi.Core.Constants;
using StocksApi.Core.Models;
using System.Text.Json;

namespace StocksApi.Core.Services;

public class StocksService : IStocksService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly string _apiToken;
    private readonly IMemoryCache _stockCache;

    public StocksService(HttpClient httpClient, ILogger logger, IOptions<AppSettings> config, IMemoryCache memoryCache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiToken = config.Value.IexApiToken;
        _stockCache = memoryCache;
    }

    public async Task<Result<StockDetail>> GetStockByTicker(string ticker)
    {
        try
        {
            _logger.LogInformation("Calling IEX API to GET stock - ticker: '{ticker}'", ticker);

            HttpResponseMessage apiResponse = await _httpClient.GetAsync(BuildGetStockUri(ticker));

            if (apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound 
                && apiResponse.ReasonPhrase == ExternalStockApiConstants.ResponseReasonPhraseUnknownTicker)
            {
                _logger.LogInformation(ApiTickerNotFoundLogMessage(ticker));
                return Result<StockDetail>.NotFoundResult($"Unknown ticker '{ticker}'");
            }

            if (!apiResponse.IsSuccessStatusCode)
            {
                _logger.LogError(await ApiUnexpectedStatusCodeLogMessage(apiResponse));
                return Result<StockDetail>.UnexpectedErrorResult($"IEX API returned status code {apiResponse.StatusCode}");
            }

            _logger.LogInformation("Received success response from IEX API for GET stock - ticker: '{ticker}'", ticker);

            var content = await apiResponse.Content.ReadAsStringAsync();

            // Parse to dynamic object and retrieve details
            var stockDto = JObject.Parse(content);
            var name = stockDto["companyName"].ToString();
            CacheStockName(ticker, name);
            var price = decimal.Parse(stockDto["latestPrice"].ToString());
            var quoteTimestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(stockDto["latestUpdate"].ToString())).DateTime;
            var stockDetail = new StockDetail(ticker, name, new Quote(price, Currencies.USDollar, quoteTimestamp));

            _logger.LogInformation("Finished retrieving stock for ticker '{ticker}': {stock}", ticker, JsonSerializer.Serialize(stockDetail));

            return Result<StockDetail>.OKResult(stockDetail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while getting stock with ticker '{ticker}'", ticker);
            return Result<StockDetail>.UnexpectedErrorResult($"Exception occurred while getting stock from IEX API");
        }
    }

    public async Task<Result<StockPriceHistory>> GetStockPriceHistory(string ticker, string range)
    {
        try
        {
            _logger.LogInformation("Calling IEX API to GET stock history - ticker: '{ticker}', range: '{range}'", ticker, range);

            HttpResponseMessage apiResponse = await _httpClient.GetAsync(BuildGetStockHistoryUri(ticker, range));

            if (apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound
                && apiResponse.ReasonPhrase == ExternalStockApiConstants.ResponseReasonPhraseUnknownTicker)
            {
                _logger.LogInformation(ApiTickerNotFoundLogMessage(ticker));
                return Result<StockPriceHistory>.NotFoundResult($"Unknown ticker '{ticker}'");
            }

            if (!apiResponse.IsSuccessStatusCode)
            {
                _logger.LogError(await ApiUnexpectedStatusCodeLogMessage(apiResponse));
                return Result<StockPriceHistory>.UnexpectedErrorResult($"IEX API returned status code {apiResponse.StatusCode}");
            }

            _logger.LogInformation("Received success response from IEX API for GET stock history - ticker: '{ticker}', range: '{range}'", ticker, range);

            // Need to get stock name separately (history response doesn't include it)
            var stockName = GetStockNameFromCache(ticker);
            if (stockName  == null)
            {
                _logger.LogInformation("No company name cached for ticker '{ticker}'", ticker);
                var companyNameResult = await GetStockCompanyName(ticker);
                if (!companyNameResult.Success)
                    return Result<StockPriceHistory>.ResultFromError(companyNameResult.Error);

                stockName = companyNameResult.Data;
            }

            var content = await apiResponse.Content.ReadAsStringAsync();

            // Parse to dynamic object and retrieve details
            var stockHistoryDto = JArray.Parse(content);
            var history = new StockPriceHistory(new Stock(ticker, stockName)); // TODO probably worth caching history too
            foreach (JObject stockPriceDto in stockHistoryDto) 
            {
                var price = decimal.Parse(stockPriceDto["close"].ToString());
                var timestamp = ParsePriceHistoryDateTime(stockPriceDto["date"].ToString(), stockPriceDto["label"].ToString());
                var quote = new Quote(price, Currencies.USDollar, timestamp);
                history.PriceHistory.Add(quote);
            }

            _logger.LogInformation("Finished retrieving stock history for ticker '{ticker}' and range '{range}': {stock}", ticker, range, JsonSerializer.Serialize(history));

            return Result<StockPriceHistory>.OKResult(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while getting stock history with ticker '{ticker}' and range '{range}'", ticker, range);
            return Result<StockPriceHistory>.UnexpectedErrorResult($"Exception occurred while getting stock history from IEX API");
        }
    }

    private async Task<Result<string>> GetStockCompanyName(string ticker)
    {
        try
        {
            _logger.LogInformation("Calling IEX API to GET company - ticker: '{ticker}'", ticker);

            HttpResponseMessage apiResponse = await _httpClient.GetAsync(BuildGetCompanyUri(ticker));

            if (apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound
                && apiResponse.ReasonPhrase == ExternalStockApiConstants.ResponseReasonPhraseUnknownTicker)
            {
                _logger.LogInformation(ApiTickerNotFoundLogMessage(ticker));
                return Result<string>.NotFoundResult($"Unknown ticker '{ticker}'");
            }

            if (!apiResponse.IsSuccessStatusCode)
            {
                _logger.LogError(await ApiUnexpectedStatusCodeLogMessage(apiResponse));
                return Result<string>.UnexpectedErrorResult($"IEX API returned status code {apiResponse.StatusCode}");
            }

            _logger.LogInformation("Received success response from IEX API for GET company - ticker: '{ticker}'", ticker);

            var content = await apiResponse.Content.ReadAsStringAsync();

            // Parse to dynamic object and retrieve details
            var companyDto = JObject.Parse(content);
            var companyName = companyDto["companyName"].ToString();
            CacheStockName(ticker, companyName);
            return Result<string>.OKResult(companyName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while getting company with ticker '{ticker}'", ticker);
            return Result<string>.UnexpectedErrorResult($"Exception occurred while getting company from IEX API");
        }
    }

    private DateTime ParsePriceHistoryDateTime(string dateValue, string dynamicValue)
    {
        var dateTime = DateTime.ParseExact(dateValue, "yyyy-MM-dd", null);

        // The dynamic value is either a time (for shorter historical ranges) or a date (for longer ranges)
        if (DateTime.TryParseExact(dynamicValue, "HH:mm", null, System.Globalization.DateTimeStyles.None, out var dt))
        {
            var timeVal = dt.TimeOfDay;
            dateTime = dateTime.Add(timeVal);
        }
        return dateTime;
    }

    // Caching get/set
    private void CacheStockName(string ticker, string name) => _stockCache.Set(ticker, name);
    private string? GetStockNameFromCache(string ticker) => _stockCache.Get<string>(ticker);

    // IEX API URI builders
    private string BuildGetStockUri(string ticker) => $"stock/{ticker}/quote?token={_apiToken}";
    private string BuildGetCompanyUri(string ticker) => $"stock/{ticker}/company?token={_apiToken}";
    private string BuildGetStockHistoryUri(string ticker, string range) => $"stock/{ticker}/chart/{range}?token={_apiToken}";

    // Log messages
    private string ApiTickerNotFoundLogMessage(string ticker) => $"IEX API returned status code NotFound. Ticker '{ticker}' unknown.";
    private async Task<string> ApiUnexpectedStatusCodeLogMessage(HttpResponseMessage apiResponse) => 
        $"IEX API returned status code {apiResponse.StatusCode}. Content: '{await apiResponse.Content.ReadAsStringAsync()}', RequestUri: '{apiResponse.RequestMessage?.RequestUri}'";
}
