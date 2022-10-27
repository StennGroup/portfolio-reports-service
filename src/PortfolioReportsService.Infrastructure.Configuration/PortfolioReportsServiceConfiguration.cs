using Seedwork.Auth.Configuration;
using Seedwork.Configuration;
using Seedwork.HttpClientHelpers;
using Seedwork.Logging;
using Seedwork.ServiceBus;
using Seedwork.Web;

namespace PortfolioReportsService.Infrastructure.Configuration
{
    public class PortfolioReportsServiceConfiguration :
        ConfigurationBaseDto,
        ILoggingConfiguration,
        IRequestResponseLoggerMiddlewareConfiguration,
        IServiceBusConfiguration,
        ILongTasksWebExecutorConfiguration
    {
        public string PortfolioReportsServiceDbContext { get; set; }

        public LoggingConfiguration LoggingConfiguration { get; set; }

        public RequestResponseLoggerMiddlewareConfig RequestResponseLoggerMiddlewareConfig { get; set; }

        public Auth0Configuration Auth0Configuration { get; set; }

        public string NServiceBusLicense { get; set; }

        public string BusTransportConnectionString { get; set; }

        public string AzureDataBusStorageConnectionString { get; set; }

        public int ServiceBusHeartBeatPeriodInMinutes { get; set; }
        public ServiceBusConfiguration ServiceBusConfiguration { get; set; }
        public string OperationsApiAdClientId { get; set; }
        public string OperationsApiAdClientSecret { get; set; }
        public string OperationsApiAdInstance { get; set; }
        public string OperationsApiAdTenantId { get; set; }
        public string RootUrlOperationsApi { get; set; }
        public BasicAuthentication BasicAuthentication { get; set; }
        public AtradiusFtpConfig FtpConfig { get; set; }
        public LongTasksWebExecutorConfig LongTasksWebExecutorConfig { get; set; }
    }
}