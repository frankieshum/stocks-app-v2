namespace StocksApi.Web.Constants
{
    public static class ApiConstants
    {
        public const string PriceHistoryRangeFiveDay = "5d";
        public const string PriceHistoryRangeOneMonth = "1m";
        public const string PriceHistoryRangeThreeMonth = "3m";
        public const string PriceHistoryRangeSixMonth = "6m";
        public const string PriceHistoryRangeYearToDate = "ytd";
        public const string PriceHistoryRangeOneYear = "1y";
        public const string PriceHistoryRangeTwoYear = "2y";
        public const string PriceHistoryRangeFiveYear = "5y";
        public const string PriceHistoryRangeMax = "max";
        public static readonly string[] PriceHistoryRangeValues =
        {
            PriceHistoryRangeFiveDay,
            PriceHistoryRangeOneMonth,
            PriceHistoryRangeThreeMonth,
            PriceHistoryRangeSixMonth,
            PriceHistoryRangeYearToDate,
            PriceHistoryRangeOneYear,
            PriceHistoryRangeTwoYear,
            PriceHistoryRangeFiveYear,
            PriceHistoryRangeMax
        };
    }
}

