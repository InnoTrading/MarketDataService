namespace MarketDataService.Domain.DTOs;

public record StockDetailsDto(
    string  Ticker,
    IEnumerable<StockHistoricalDataPointDto> HistoricalDataPoints);