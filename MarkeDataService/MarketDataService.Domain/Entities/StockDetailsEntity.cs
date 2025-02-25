namespace MarketDataService.Domain.Entities;

public record StockDetailsEntity(
    string  Ticker,
    IEnumerable<StockHistoricalDataPoint> HistoricalDataPoints);