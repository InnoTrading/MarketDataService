using MarketDataService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketDataController(IMarketDataService marketDataService) : ControllerBase
    {
        private readonly IMarketDataService _marketDataService = marketDataService;

        [HttpGet("popular-stocks")]
        public async Task<IActionResult> GetPopularStocks([FromQuery] string interval)
        {
            var result = await _marketDataService.GetPopularStocksAsync(interval);

            if (!result.Any())
                return NotFound();
            return Ok(result);
        }
    }
}