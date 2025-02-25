namespace MarketDataService.Domain.DTOs;
public record StockHistoricalDataPointDto(
    DateTime Time,
    decimal OpenPrice,
    decimal HighPrice,
    decimal LowPrice,
    decimal ClosePrice,
    long Volume);
