using MarketDataService.Domain.Entities;

namespace MarketDataService.Application.Interfaces;

public interface IMarketDataService
{
    Task<List<StockInfoEntity>> GetPopularStocksAsync(int interval);
    Task<StockDetailsEntity?> GetStockDetailsAsync(string ticker, int interval);
}