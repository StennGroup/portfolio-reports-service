using System;
using System.Threading.Tasks;
using Seedwork.UnitOfWork;

namespace PortfolioReportsService.UnitTests
{
    public class MockTaskExecutor : ITaskExecutor
    {
        public async Task<bool> Execute(Func<Task<bool>> task)
        {
            return await task.Invoke();
        }
    }
}