using System;
using Seedwork.ServiceBus;
using Seedwork.Web.ServiceBus.Configuration;
using PortfolioReportsService.Contracts;
using PortfolioReportsService.Infrastructure.Configuration;

namespace PortfolioReportsService.Infrastructure.ServiceBus
{
    public class PortfolioReportsServiceProcessingEndpointConfigurator : ProcessingEndpointConfigurator<PortfolioReportsServiceConfiguration, ServiceBusLogContext>
    {
        public override string EndpointName => PortfolioReportsServiceQueue.Name;
        protected override int ImmediateRetriesCount => 2;
        protected override int DelayedRetriesCount => 2;
        protected override TimeSpan DelayedRetryTimeIncrease => TimeSpan.FromSeconds(2);
        protected override int MessageProcessingConcurrency => 10;
        protected override TimeSpan TimeToKeepOutboxDeduplicationData => TimeSpan.FromHours(1);
        public override SubscriptionStrategy SubscriptionStrategy => SubscriptionStrategy.All();
    }
}