using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seedwork.ServiceBus;
using Seedwork.UnitOfWork;
using Seedwork.Web;
using Seedwork.Web.ServiceBus;
using Seedwork.Web.ServiceBus.Configuration;
using Serilog;
using PortfolioReportsService.Infrastructure.Configuration;
using PortfolioReportsService.Infrastructure.ServiceBus;
using PortfolioReportsService.Persistence.Write;
using System.Linq;

namespace PortfolioReportsService.WebApp
{
    public class PortfolioReportsServiceServiceHost : SeedworkHostWithServiceBus<Startup, PortfolioReportsServiceConfiguration, RequestLogContext, ServiceBusLogContext>
    {
        protected override IEnumerable<Func<IServiceProvider, Task>> GetBeforeStartupActions()
            => Enumerable.Empty<Func<IServiceProvider, Task>>();

        protected override IConfigureServiceBusEndpoint<PortfolioReportsServiceConfiguration> EndpointInWebContainerConfigurator => new PortfolioReportsServiceSendOnlyEndpointConfigurator();

        protected override IEnumerable<IConfigureProcessingServiceBusEndpoint<PortfolioReportsServiceConfiguration>> EndpointsInSeparateContainersConfigurators =>
            new[] { new PortfolioReportsServiceProcessingEndpointConfigurator() };
    }
}