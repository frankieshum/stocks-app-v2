using Microsoft.AspNetCore.Mvc;
using StocksApi.Core.Models;
using StocksApi.Core.Services;
using StocksApi.Web.Mappings;
using StocksApi.Web.Validation;
using System.ComponentModel.DataAnnotations;

namespace StocksApi.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StocksController : BaseController
{
    private readonly IStocksService _stocksService;
    private readonly ILogger _logger;

    public StocksController(IStocksService stocksService, ILogger logger)
    {
        _stocksService = stocksService;
        _logger = logger;
    }

    [HttpGet("{ticker}")]
    [ProducesResponseType(typeof(List<StockDetail>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> GetByTicker([FromRoute] string ticker)
    {
        _logger.LogInformation($"Getting stock - ticker: '{ticker}'");

        var stockResult = await _stocksService.GetStockByTicker(ticker.ToUpper());

        if (stockResult.IsNotFoundError())
            return NotFoundResult(); 

        if (!stockResult.Success)
            return InternalServerErrorResult();

        return Ok(stockResult.Data);
    }

    [HttpGet("{ticker}/history")]
    [ProducesResponseType(typeof(StockPriceHistory), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> GetPriceHistory([FromRoute] string ticker, [FromQuery, Required, ValidPriceHistoryRange] string range)
    {
        _logger.LogInformation($"Getting stock price history - ticker: '{ticker}', range: '{range}'");

        var priceHistoryResult = await _stocksService.GetStockPriceHistory(ticker.ToUpper(), PriceHistoryMapper.MapRangeValue(range));

        if (priceHistoryResult.IsNotFoundError())
            return NotFoundResult();

        if (!priceHistoryResult.Success)
            return InternalServerErrorResult();

        return Ok(priceHistoryResult.Data);
    }
}
