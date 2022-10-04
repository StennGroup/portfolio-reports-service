using System.Threading.Tasks;
using Seedwork.ServiceBus;
using Seedwork.UnitOfWork;
using Serilog;
using PortfolioReportsService.Contracts.Events;

namespace PortfolioReportsService.WebApp.Handlers
{
    public class TestEventHandler : TransactionMessageHandler<TestEvent>
    {
        private readonly ILogger _logger;

        public TestEventHandler(ITaskExecutor taskExecutor, ILogger logger) : base(taskExecutor, logger)
        {
            _logger = logger;
        }

        protected override Task HandleInTransaction(TestEvent message)
        {
            _logger.Debug(message.Test);
            return Task.CompletedTask;
        }
    }
}