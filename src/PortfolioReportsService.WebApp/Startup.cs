using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seedwork.Auth.Configuration;
using Seedwork.Auth.Token;
using Seedwork.Auth.Utils;
using Seedwork.Configuration;
using Seedwork.UnitOfWork.DependencyInjection;
using Seedwork.Web;
using Seedwork.Web.Extensions;
using Seedwork.Web.HealthChecks;
using Seedwork.Web.Middleware;
using PortfolioReportsService.Application;
using PortfolioReportsService.Application.Port;
using PortfolioReportsService.Infrastructure;
using PortfolioReportsService.Infrastructure.Configuration;
using PortfolioReportsService.Infrastructure.Web;
using PortfolioReportsService.Persistence;
using PortfolioReportsService.Persistence.Write;

namespace PortfolioReportsService.WebApp
{
    public class Startup : SeedworkStartup<PortfolioReportsServiceConfiguration, RequestLogContext>
    {
        private const string AllowAllCorsPolicy = "AllowAll";

        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void DoConfigureServices(IServiceCollection services)
        {
            var serviceConfiguration = Configuration.Get<PortfolioReportsServiceConfiguration>();
            services.RemoveRoutingNotForProduction(ConfigurationDto.ConfigType == ConfigType.Prod);

            ConfigureAuthentication(services, serviceConfiguration.Auth0Configuration,
                serviceConfiguration.ConfigType == ConfigType.Dev);

            services.AddScoped<SecurityContextProvider>();
            services.AddScoped<IUserContext>(p => p.GetRequiredService<SecurityContextProvider>().Context);

            services.AddHttpContextAccessor();

            services.AddControllers(options =>
                {
                    options.Filters.Add<PerformanceLoggerFilter>();
                    options.Filters.Add<TransactionControlFilter>();
                    options.Filters.Add(new AuthorizeFilter());
                })
                .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                           ForwardedHeaders.XForwardedProto;
                // Only loopback proxies are allowed by default.
                // Clear that restriction because forwarders are enabled by explicit 
                // configuration.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllCorsPolicy,
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddUnitOfWork<PortfolioReportsServiceDbContext>();
            services.AddTaskExecutor();

            services.AddApiVersion();
            if (ConfigurationDto.ConfigType != ConfigType.Prod)
                services.AddSwagger();
            services
                .AddApplicationServices()
                .AddRepositories();
        }

        protected override void AddHealthChecks(IHealthChecksBuilder builder)
        {
           
        }

        protected override void DoConfigure(IApplicationBuilder builder)
        {
            builder.UseForwardedHeaders();
            builder.UseRouting();
            builder.UseCors(AllowAllCorsPolicy);
            if (ConfigurationDto.ConfigType == ConfigType.Dev)
                builder.UseHsts();
            if (ConfigurationDto.ConfigType != ConfigType.Prod)
                builder.UseSwaggerWithApiVersions();
            builder.UseAuthentication();
            builder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        protected override (bool Enabled, IReadOnlyCollection<string> Excludes) RequestResponseLogging => (true, new List<string>()
        {
            //swagger
            @"\/index\.html.*",
            @"\/swagger.*",
            //api
            @"\/\b",
            @"\/favicon.*"
        });

        private static void ConfigureAuthentication(IServiceCollection services, Auth0Configuration configuration,
            bool development = true)
        {
            var builder = AuthBuilder
                .CreateDefaultAuth0BearerAuthentication(
                    services,
                    configuration.Auth0Domain,
                    configuration.Auth0Audience,
                    configuration.Auth0OpenIdConnectConfigurationEndpoint)
                .AddAuthLogger<AuthLogger>()
                .AddTokenExtractor<DefaultTokenExtractor>()
                .AddTokenValidator<DefaultTokenValidator>();

            if (development)
            {
                builder
                    .AddTokenValidator<OfflineTokenValidator>()
                    .DisableOnBuildOpenIdConfigurationEndpointCheck();
            }

            builder
                .AddDefaultUserWithoutAuthorizationFlowHandler()
                .Build();
        }

        private void ConfigureDbContexts(IServiceCollection services)
        {
            services.AddDbContext<PortfolioReportsServiceDbContext>(options =>
            {
                options
                    .UseSqlServer(ConfigurationDto.PortfolioReportsServiceDbContext);
            });
        }
    }
}