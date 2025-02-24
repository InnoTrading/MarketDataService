namespace MarketDataService.Domain.Entities
{
    public class StockHistoricalDataPoint
    {
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public long Volume { get; set; }
    }
}