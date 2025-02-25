namespace MarketDataService.Domain.Entities;

public record StockHistoricalDataPoint(
    DateTime Time,
    decimal OpenPrice,
    decimal HighPrice,
    decimal LowPrice,
    decimal ClosePrice,
    long Volume);