using MarketDataService.Domain.DTOs;

namespace MarketDataService.Domain.Interfaces;

public interface IMarketDataAdapter
{
    Task<List<StockInfoDto>> GetPopularStocksAsync(int interval);
    Task<StockDetailsDto?> GetStockDetailsAsync(string ticker, int interval);
    Task<StockPriceDto> GetStockActualPriceAsync(string ticker);
    Task<List<StockNameDto>> GetStocksByFilter(string filter);
}