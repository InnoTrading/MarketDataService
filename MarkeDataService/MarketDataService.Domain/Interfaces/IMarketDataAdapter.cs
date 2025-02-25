using System.Security;
using MarketDataService.Domain.Entities;

namespace MarketDataService.Domain.Interfaces;

public interface IMarketDataAdapter
{
    Task<List<StockInfoEntity>> GetPopularStocksAsync(int interval);
    Task<StockDetailsEntity?> GetStockDetailsAsync(string ticker, int interval);
}