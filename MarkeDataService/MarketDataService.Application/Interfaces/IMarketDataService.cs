using MarketDataService.Domain.DTOs;

namespace MarketDataService.Application.Interfaces;

public interface IMarketDataService
{
    Task<List<StockInfoDto>> GetPopularStocksAsync(int interval);
    Task<StockDetailsDto?> GetStockDetailsAsync(string ticker, int interval);
    Task<StockPriceDto> GetActualStockPriceAsync(string ticker);
}