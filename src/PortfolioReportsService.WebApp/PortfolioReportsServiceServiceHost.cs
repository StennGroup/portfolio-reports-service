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

namespace PortfolioReportsService.WebApp
{
    public class PortfolioReportsServiceServiceHost : SeedworkHostWithServiceBus<Startup, PortfolioReportsServiceConfiguration, RequestLogContext, ServiceBusLogContext>
    {
        protected override IEnumerable<Func<IServiceProvider, Task>> GetBeforeStartupActions()
        {
            Func<IServiceProvider, Task> res = async services =>
            {
                using (var scope = services.CreateScope())
                using (var context = scope.ServiceProvider.GetRequiredService<PortfolioReportsServiceDbContext>())
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger>();

                    EnvironmentChecker.CheckTheSafetyOfMigrations(context.Database.GetDbConnection().ConnectionString
                        , scope.ServiceProvider.GetRequiredService<IConfiguration>());
                    try
                    {
                        await context.Database.MigrateAsync();
                        logger.Information("Migrations applied");
                    }
                    catch (Exception e)
                    {
                        logger.Fatal(e, "Unable to apply migrations.");
                        throw; // here we can throw and log.Fatal at the same time, because the server is not started
                    }
                }
            };
            return new[] { res };
        }

        protected override IConfigureServiceBusEndpoint<PortfolioReportsServiceConfiguration> EndpointInWebContainerConfigurator => new PortfolioReportsServiceSendOnlyEndpointConfigurator();

        protected override IEnumerable<IConfigureProcessingServiceBusEndpoint<PortfolioReportsServiceConfiguration>> EndpointsInSeparateContainersConfigurators =>
            new[] { new PortfolioReportsServiceProcessingEndpointConfigurator() };
    }
}