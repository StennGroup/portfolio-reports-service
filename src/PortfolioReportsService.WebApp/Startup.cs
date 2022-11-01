using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using idunno.Authentication.Basic;
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
using PortfolioReportsService.Application.Interfaces;
using PortfolioReportsService.Application.Port;
using PortfolioReportsService.Application.Services;
using PortfolioReportsService.Infrastructure;
using PortfolioReportsService.Infrastructure.Configuration;
using PortfolioReportsService.Infrastructure.OperationsApi;
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

            ConfigureAuthentication(services);

            services.AddScoped<SecurityContextProvider>();
            services.AddScoped<IUserContext>(p => p.GetRequiredService<SecurityContextProvider>().Context);
            
            services.AddOperationsClient(serviceConfiguration);

            services.AddHttpContextAccessor();

            services.AddControllers(options =>
                {
                    options.Filters.Add<PerformanceLoggerFilter>();
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
            

            services.AddApiVersion();
            if (ConfigurationDto.ConfigType != ConfigType.Prod)
                services.AddSwagger();
            services
                .AddApplicationServices();
            
            services.AddScoped<LongTaskWebExecutor>();
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

        private void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasic(options =>
                {
                    options.Realm = "Operations Notifier API";
                    options.AllowInsecureProtocol = false;
                    options.Events = new BasicAuthenticationEvents
                    {
                        OnValidateCredentials = ctx =>
                        {
                            if (ctx.Username == ConfigurationDto.BasicAuthentication.Login &&
                                ctx.Password == ConfigurationDto.BasicAuthentication.Password)
                            {
                                var claims = new[]
                                {
                                    new Claim(ClaimTypes.NameIdentifier, ctx.Username, ClaimValueTypes.String,
                                        ctx.Options.ClaimsIssuer),
                                    new Claim(ClaimTypes.Name, ctx.Username, ClaimValueTypes.String,
                                        ctx.Options.ClaimsIssuer)
                                };
                                ctx.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, ctx.Scheme.Name));
                                ctx.Success();
                            }
                            else
                            {
                                ctx.Fail("Invalid login or password");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}