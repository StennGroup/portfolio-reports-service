using System.Threading.Tasks;
using Seedwork.ServiceBus;
using Seedwork.UnitOfWork;
using Serilog;
using PortfolioReportsService.Contracts.Commands;

namespace PortfolioReportsService.WebApp.Handlers
{
    public class TestCommandHandler : TransactionMessageHandler<TestCommand>
    {
        private readonly ILogger _logger;

        public TestCommandHandler(ITaskExecutor taskExecutor, ILogger logger) : base(taskExecutor, logger)
        {
            _logger = logger;
        }

        protected override Task HandleInTransaction(TestCommand message)
        {
            _logger.Debug(message.Test);
            return Task.CompletedTask;
        }
    }
}