using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PortfolioReportsService.Application;

namespace PortfolioReportsService.UnitTests.Application
{
    [TestFixture]
    public sealed class MapperConfigurationTests
    {
        [Test]
        public void ValidateMappingConfiguration()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.BuildServiceProvider()
                .GetRequiredService<IMapper>()
                .ConfigurationProvider
                .AssertConfigurationIsValid();
        }
    }
}