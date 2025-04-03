using MarketDataService.Domain.DTOs;
using MarketDataService.Application.Interfaces;
using MarketDataService.Domain.Interfaces;

namespace MarketDataService.Application.Services;

public class MarketDataService(IMarketDataAdapter marketDataAdapter) : IMarketDataService
{
    private readonly IMarketDataAdapter _marketDataAdapter = marketDataAdapter;

    public async Task<List<StockInfoDto>> GetPopularStocksAsync(int interval)
    {
        return await _marketDataAdapter.GetPopularStocksAsync(interval);
    }

    public async Task<StockDetailsDto?> GetStockDetailsAsync(string ticker, int interval)
    {
        return await _marketDataAdapter.GetStockDetailsAsync(ticker, interval);
    }

    public async Task<StockPriceDto> GetActualStockPriceAsync(string ticker)
    {
        return await _marketDataAdapter.GetStockActualPriceAsync(ticker);
    }

    public async Task<List<StockNameDto>> GetStocksByFilter(string filter)
    {
        return await _marketDataAdapter.GetStocksByFilter(filter);
    }
}