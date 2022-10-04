using Seedwork.Messaging.Contracts;

namespace PortfolioReportsService.Contracts.Commands
{
    public class TestCommand : ICommand
    {
        public string Test { get; set; }
        public string Destination => PortfolioReportsServiceQueue.Name;
    }
}