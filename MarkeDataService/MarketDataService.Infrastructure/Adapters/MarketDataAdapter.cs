using MarketDataService.Domain.DTOs;
using MarketDataService.Domain.Interfaces;
using System.Globalization;
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

    public async Task<List<StockInfoDto>> GetPopularStocksAsync(int interval = 5)
    {
        var result = new List<StockInfoDto>();
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
                                result.Add(new StockInfoDto(
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

    public async Task<StockDetailsDto?> GetStockDetailsAsync(string ticker, int interval = 5)
    {
        var response = await FetchData(ticker, interval);
        if (!response.IsSuccessStatusCode)
            return null;

        string json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        string intervalString = $"{interval}min";

        if (!doc.RootElement.TryGetProperty($"Time Series ({intervalString})", out JsonElement timeSeries))
            return null;

        var dataPoints = timeSeries.EnumerateObject()
            .Select(entry => ParseDataPoint(entry))
            .Where(dp => dp != null)
            .Select(dp => dp!)
            .ToList();

        return dataPoints.Any() ? new StockDetailsDto(ticker, dataPoints) : null;
    }

    private StockHistoricalDataPointDto? ParseDataPoint(JsonProperty entry)
    {
        if (!DateTime.TryParse(entry.Name, out DateTime dateTimeParsed))
            return null;

        var data = entry.Value;
        if (!data.TryGetProperty("1. open", out JsonElement openPriceElement) ||
            !data.TryGetProperty("2. high", out JsonElement highPriceElement) ||
            !data.TryGetProperty("3. low", out JsonElement lowPriceElement) ||
            !data.TryGetProperty("4. close", out JsonElement closePriceElement) ||
            !data.TryGetProperty("5. volume", out JsonElement volumeElement))
            return null;

        if (!decimal.TryParse(openPriceElement.GetString(), out decimal openPrice) ||
            !decimal.TryParse(highPriceElement.GetString(), out decimal highPrice) ||
            !decimal.TryParse(lowPriceElement.GetString(), out decimal lowPrice) ||
            !decimal.TryParse(closePriceElement.GetString(), out decimal closePrice) ||
            !long.TryParse(volumeElement.GetString(), out long volume))
            return null;

        return new StockHistoricalDataPointDto(dateTimeParsed, openPrice, highPrice, lowPrice, closePrice, volume);
    }

    public async Task<StockPriceDto> GetStockActualPriceAsync(string ticker)
    {
        string url = $"https://finnhub.io/api/v1/quote?symbol={ticker}&token={_apiKey}";

        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            if (!doc.RootElement.TryGetProperty("c", out JsonElement currentPriceElement))
                throw new Exception("Failed to fetch stock data");

            string priceString = currentPriceElement.GetRawText();
            if (!decimal.TryParse(priceString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                throw new Exception("Failed to parse data.");

            return new StockPriceDto(ticker, price);
        }
    }
    public async Task<List<StockNameDto>> GetStocksByFilter(string filter)
    {
        string exchange = "US";
        string url = $"https://finnhub.io/api/v1/stock/symbol?exchange={exchange}&token={_apiKey}";

        var result = new List<StockNameDto>();

        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                if (element.TryGetProperty("description", out JsonElement descriptionElement) &&
                    element.TryGetProperty("symbol", out JsonElement symbolElement))
                {
                    var description = descriptionElement.GetString()!;
                    var symbol = symbolElement.GetString()!;

                    // Je�li filtr jest pusty, zwracamy wszystkie pozycje
                    // lub sprawdzamy, czy filtr wyst�puje w opisie lub symbolu (ignoruj�c wielko�� liter)
                    if (string.IsNullOrWhiteSpace(filter) ||
                        description.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        symbol.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        result.Add(new StockNameDto(description, symbol));
                    }
                }
            }
        }
        return result;
    }


}