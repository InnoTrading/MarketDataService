namespace MarketDataService.Domain.DTOs;

public record StockInfoDto(string Ticker, string Name, decimal CurrentPrice, long DayVolume);