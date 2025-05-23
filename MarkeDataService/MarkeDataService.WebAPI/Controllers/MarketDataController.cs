﻿using MarketDataService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetActualPriceAsync(string ticker)
    {
        var result = await _marketDataService.GetActualStockPriceAsync(ticker);
        if(result == null)
            return NotFound();
        
        return Ok(result);
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