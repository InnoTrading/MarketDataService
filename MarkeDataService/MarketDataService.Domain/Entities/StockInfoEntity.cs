namespace MarketDataService.Domain.Entities
{
    public class StockInfoEntity
    {
        public string? Ticker { get; set; }
        public string? Name { get; set; }
        public decimal CurrentPrice { get; set; }
        public long Volume {  get; set; }
    }
}
