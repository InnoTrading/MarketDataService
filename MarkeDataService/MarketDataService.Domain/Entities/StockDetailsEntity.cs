namespace MarketDataService.Domain.Entities
{
    public class StockDetailsEntity
    {
        public StockInfoEntity StockInfo { get; set; }
        public IEnumerable<StockHistoricalDataPoint> HistoricalDataPoints { get; set; }
    }
}