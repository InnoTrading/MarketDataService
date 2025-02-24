using MarketDataService.Domain.Entities;

namespace MarketDataService.Domain.Interfaces
{
    public interface IMarketDataAdapter
    {
        Task<List<StockInfoEntity>> GetPopularStocksAsync(string interval);
        //Task<StockDetailsEntity> GetStockDetailsAsync(string ticker);
    }
}
