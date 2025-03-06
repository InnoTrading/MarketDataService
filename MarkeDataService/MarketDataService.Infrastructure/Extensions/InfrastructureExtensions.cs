using MarketDataService.Domain.Interfaces;
using MarketDataService.Infrastructure.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketDataService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient();
        string apiKey = configuration["FinnHub:ApiKey"]!;
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("Api key not found FinnHub:ApiKey");
        }

        services.AddScoped<IMarketDataAdapter>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            return new MarketDataAdapter(httpClient, apiKey);
        });

        return services;
    }
}