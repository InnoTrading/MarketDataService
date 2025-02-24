using MarketDataService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MarketDataService.Application.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IMarketDataService, Services.MarketDataService>();
            return services;
        }
    }
}