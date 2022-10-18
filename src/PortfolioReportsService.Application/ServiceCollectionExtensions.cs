using Microsoft.Extensions.DependencyInjection;
using PortfolioReportsService.Application.Interfaces;
using PortfolioReportsService.Application.Services;

namespace PortfolioReportsService.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IAtradiusReportGenerator, AtradiusReportGenerator>();
            serviceCollection.AddScoped<IAtradiusReportService, AtradiusReportService>();
            serviceCollection.AddAutoMapper(typeof(AutoMapperProfile));
            return serviceCollection;
        }
    }
}