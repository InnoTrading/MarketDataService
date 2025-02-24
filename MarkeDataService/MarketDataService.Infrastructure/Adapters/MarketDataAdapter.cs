using MarketDataService.Domain.Entities;
using MarketDataService.Domain.Interfaces;
using System.Text.Json;

namespace MarketDataService.Infrastructure.Adapters
{
    public class MarketDataAdapter(HttpClient httpClient, string apiKey) : IMarketDataAdapter
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _apiKey = apiKey;

        private readonly List<(string Ticker, string Name)> _popularStocks = new List<(string, string)>
        {
            ("AAPL", "Apple Inc."),
            ("MSFT", "Microsoft Corporation"),
            ("GOOGL", "Alphabet Inc."),
            ("AMZN", "Amazon.com Inc."),
            ("TSLA", "Tesla Inc."),
            ("NVDA", "Nvidia Corporation")
        };

        private async Task<HttpResponseMessage> FetchData(string ticker, int interval)
        {
            string intervalString = interval + "min";
            var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&" +
                      $"symbol={ticker}&interval={intervalString}&apikey={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            
            return response;
        }

        public async Task<List<StockInfoEntity>> GetPopularStocksAsync(int interval = 5)
        {
            var result = new List<StockInfoEntity>();


            foreach (var stock in _popularStocks)
            {
                var response = await FetchData(stock.Ticker, interval);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(json);

                    if (doc.RootElement.TryGetProperty($"Time Series ({interval})", out JsonElement timeSeries))
                    {
                        var enumerator = timeSeries.EnumerateObject();
                        if (enumerator.MoveNext())
                        {
                            var latestData = enumerator.Current.Value;
                            if (latestData.TryGetProperty("4. close", out JsonElement closePriceElement) &&
                                latestData.TryGetProperty("5. volume", out JsonElement volumeElement))
                            {
                                if (decimal.TryParse(closePriceElement.GetString(), out decimal price) &&
                                    long.TryParse(volumeElement.GetString(), out long volume))
                                {
                                    result.Add(new StockInfoEntity
                                    {
                                        Ticker = stock.Ticker,
                                        Name = stock.Name,
                                        CurrentPrice = price,
                                        Volume = volume
                                    });
                                }
                            }
                        }
                    }
                }
            }
            
            return result;
        }

        public async Task<StockDetailsEntity> GetStockDetailsAsync(string ticker, int interval = 5)
        {
            var result = new StockDetailsEntity();

            var response = await FetchData(ticker, interval);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(json);

                doc.

                if (doc.RootElement.TryGetProperty($"Time Series ({interval})", out JsonElement timeSeries))
                {
                    var enumerator = timeSeries.EnumerateObject();
                    if (enumerator.MoveNext())
                    {
                        var latestData = enumerator.Current.Value;
                        if (latestData.TryGetProperty("4. close", out JsonElement closePriceElement) &&
                            latestData.TryGetProperty("5. volume", out JsonElement volumeElement))
                        {
                            if (decimal.TryParse(closePriceElement.GetString(), out decimal price) &&
                                long.TryParse(volumeElement.GetString(), out long volume))
                            {
                              
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}