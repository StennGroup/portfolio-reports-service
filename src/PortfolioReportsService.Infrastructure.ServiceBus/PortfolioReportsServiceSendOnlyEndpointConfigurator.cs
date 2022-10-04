using Seedwork.Web.ServiceBus.Configuration;
using PortfolioReportsService.Contracts;
using PortfolioReportsService.Infrastructure.Configuration;

namespace PortfolioReportsService.Infrastructure.ServiceBus
{
    public class PortfolioReportsServiceSendOnlyEndpointConfigurator : SendOnlyEndpointConfigurator<PortfolioReportsServiceConfiguration>
    {
        public override string EndpointName => PortfolioReportsServiceQueue.Name;
    }
}