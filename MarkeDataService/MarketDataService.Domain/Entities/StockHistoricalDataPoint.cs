namespace MarketDataService.Domain.Entities
{
    public class StockHistoricalDataPoint
    {
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
    }
}