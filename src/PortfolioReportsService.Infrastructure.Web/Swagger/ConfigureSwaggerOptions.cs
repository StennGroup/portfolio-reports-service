using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Stenn.AspNetCore.Versioning;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PortfolioReportsService.Infrastructure.Web.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionInfoProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        public ConfigureSwaggerOptions(IApiVersionInfoProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });

            foreach (var versionInfo in _provider.Versions)
            {
                options.SwaggerDoc(versionInfo.RoutePathName, CreateInfoForApiVersion(versionInfo));
            }

            // add a custom operation filter which sets default values
            options.OperationFilter<SwaggerDefaultValues>();

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionInfo versionInfo)
        {
            var info = new OpenApiInfo
            {
                Title = "Onboarding API",
                Version = versionInfo.Version.ToString()
            };

            if (versionInfo.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}