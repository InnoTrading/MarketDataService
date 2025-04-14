using MarketDataService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading;
using YahooFinanceApi;

namespace MarketDataService.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketDataController(IMarketDataService marketDataService) : ControllerBase
{
    private readonly IMarketDataService _marketDataService = marketDataService;

    [HttpGet("popular-stocks")]
    public async Task<IActionResult> GetPopularStocks([FromQuery] int interval)
    {
        var result = await _marketDataService.GetPopularStocksAsync(interval);

        if (!result.Any())
            return NotFound();

        return Ok(result);
    }

    [HttpGet("stock/{ticker}")]
    public async Task<IActionResult> GetStockDetailsAsync([FromRoute] string ticker, [FromQuery] int interval)
    {
        var result = await _marketDataService.GetStockDetailsAsync(ticker, interval);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("price/{ticker}")]
    public async Task GetActualPriceAsync(CancellationToken cancellationToken, string ticker)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        while (!cancellationToken.IsCancellationRequested)
        {
            var currentPrice = await _marketDataService.GetActualStockPriceAsync(ticker);

            var jsonData = JsonConvert.SerializeObject(new { price = currentPrice });

            await Response.WriteAsync($"data: {jsonData}\n\n");

            await Response.Body.FlushAsync();

            await Task.Delay(500, cancellationToken);
        }
    }

    [HttpGet("stocks")]
    public async Task<IActionResult> GetStocksByFilter([FromQuery] string filter)
    {
        var result = await _marketDataService.GetStocksByFilter(filter);
        if(result.Any())
            return Ok(result);

        return NotFound();
    }
}