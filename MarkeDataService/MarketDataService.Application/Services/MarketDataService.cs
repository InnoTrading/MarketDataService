using MarketDataService.Application.Interfaces;
using MarketDataService.Domain.Entities;
using MarketDataService.Domain.Interfaces;

namespace MarketDataService.Application.Services;

public class MarketDataService(IMarketDataAdapter marketDataAdapter) : IMarketDataService
{
    private readonly IMarketDataAdapter _marketDataAdapter = marketDataAdapter;

    public async Task<List<StockInfoEntity>> GetPopularStocksAsync(int interval)
    {
        return await _marketDataAdapter.GetPopularStocksAsync(interval);
    }

    public async Task<StockDetailsEntity?> GetStockDetailsAsync(string ticker, int interval)
    {
        return await _marketDataAdapter.GetStockDetailsAsync(ticker, interval);
    }
}