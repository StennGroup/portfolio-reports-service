using Seedwork.Messaging.Contracts;

namespace PortfolioReportsService.Contracts.Events
{
    public class TestEvent : IEvent
    {
        public string Test { get; set; }
    }
}