namespace MarketDataService.Domain.Entities
{
    public record StockInfoEntity(string Ticker, string Name, decimal CurrentPrice, long DayVolume);
}