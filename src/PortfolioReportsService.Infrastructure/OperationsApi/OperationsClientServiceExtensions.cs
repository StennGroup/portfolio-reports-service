using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using OperationsNotifications.Infrastructure.OperationsApi;
using Polly;
using Polly.Extensions.Http;
using PortfolioReportService.InteropAbstractions.OperationsApi;
using PortfolioReportsService.Infrastructure.Configuration;
using Seedwork.HttpClientHelpers;

namespace PortfolioReportsService.Infrastructure.OperationsApi;

public static class OperationsClientServiceExtensions
{
    public static IServiceCollection AddOperationsClient(this IServiceCollection services,
        PortfolioReportsServiceConfiguration configuration)
    {
        services.AddTransient<OperationsApiAuthenticationHandler>();
        services.AddTransient<HttpLoggingHandler>();
        services.AddHttpClient<IOperationsApi, OperationsApi>(
                client => client.BaseAddress = new Uri($"{configuration.RootUrlOperationsApi}/"))
            .AddHttpMessageHandler<OperationsApiAuthenticationHandler>()
            .AddHttpMessageHandler<HttpLoggingHandler>();
        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                retryAttempt)));
    }
}