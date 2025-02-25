using MarketDataService.Domain.Entities;
using MarketDataService.Domain.Interfaces;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MarketDataService.Infrastructure.Adapters;

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
        string intervalString = $"{interval}min";

        foreach (var stock in _popularStocks)
        {
            var response = await FetchData(stock.Ticker, interval);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty($"Time Series ({intervalString})", out JsonElement timeSeries))
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
                                result.Add(new StockInfoEntity(
                                    stock.Ticker,
                                    stock.Name,
                                    price,
                                    volume
                                ));
                            }
                        }
                    }
                }
            }
        }

        return result;
    }

    public async Task<StockDetailsEntity?> GetStockDetailsAsync(string ticker, int interval = 5)
    {
        var dataPoints = new List<StockHistoricalDataPoint>();
        var response = await FetchData(ticker, interval);


        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(json);
            string intervalString = $"{interval}min";

            if (doc.RootElement.TryGetProperty($"Time Series ({intervalString})", out JsonElement timeSeries))
            {
                var enumerator = timeSeries.EnumerateObject();

                while (enumerator.MoveNext())
                {
                    var data = enumerator.Current.Value;

                    if (data.TryGetProperty("1. open", out JsonElement openPriceElement) &&
                        data.TryGetProperty("2. high", out JsonElement highPriceElement) &&
                        data.TryGetProperty("3. low", out JsonElement lowPriceElement) &&
                        data.TryGetProperty("4. close", out JsonElement closePriceElement) &&
                        data.TryGetProperty("5. volume", out JsonElement volumeElement))
                    {
                        if (decimal.TryParse(openPriceElement.GetString(), out decimal openPrice) &&
                            decimal.TryParse(highPriceElement.GetString(), out decimal highPrice) &&
                            decimal.TryParse(lowPriceElement.GetString(), out decimal lowPrice) &&
                            decimal.TryParse(closePriceElement.GetString(), out decimal closePrice) &&
                            long.TryParse(volumeElement.GetString(), out long volume))
                        {
                            if (DateTime.TryParse(enumerator.Current.Name, out DateTime dateTimeParsed))
                            {
                                dataPoints.Add(new StockHistoricalDataPoint(dateTimeParsed, openPrice, highPrice,
                                    lowPrice, closePrice, volume));
                            }
                        }
                    }
                }

                var result = new StockDetailsEntity(ticker, dataPoints);
                return result;
            }
        }

        return null!;
    }
}