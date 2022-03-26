using StocksApi.Core.Constants;
using StocksApi.Web.Constants;

namespace StocksApi.Web.Mappings
{
    public static class PriceHistoryMapper
    {
        private static readonly Dictionary<string, string> _rangeValueMappings = new Dictionary<string, string>
        {
            { ApiConstants.PriceHistoryRangeFiveDay, ExternalStockApiConstants.PriceHistoryRangeFiveDay },
            { ApiConstants.PriceHistoryRangeOneMonth, ExternalStockApiConstants.PriceHistoryRangeOneMonth },
            { ApiConstants.PriceHistoryRangeThreeMonth, ExternalStockApiConstants.PriceHistoryRangeThreeMonth },
            { ApiConstants.PriceHistoryRangeSixMonth, ExternalStockApiConstants.PriceHistoryRangeSixMonth },
            { ApiConstants.PriceHistoryRangeOneYear, ExternalStockApiConstants.PriceHistoryRangeOneYear },
            { ApiConstants.PriceHistoryRangeTwoYear, ExternalStockApiConstants.PriceHistoryRangeTwoYear },
            { ApiConstants.PriceHistoryRangeFiveYear, ExternalStockApiConstants.PriceHistoryRangeFiveYear },
            { ApiConstants.PriceHistoryRangeYearToDate, ExternalStockApiConstants.PriceHistoryRangeYearToDate },
            { ApiConstants.PriceHistoryRangeMax, ExternalStockApiConstants.PriceHistoryRangeMax }
        };

        public static string MapRangeValue(string value)
        {
            if (!_rangeValueMappings.TryGetValue(value, out var mappedValue))
                throw new KeyNotFoundException($"Unknown price history range value '{value}'");

            return mappedValue;
        }
    }
}
