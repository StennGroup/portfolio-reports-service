using Microsoft.Extensions.DependencyInjection;

namespace PortfolioReportsService.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}