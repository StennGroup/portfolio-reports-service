using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Seedwork.Logging.Enrichers;
using Seedwork.UnitOfWork;
using Seedwork.UnitOfWork.DependencyInjection;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using PortfolioReportsService.Application;
using PortfolioReportsService.Persistence.Write;
using PortfolioReportsService.UnitTests.Tools;

namespace PortfolioReportsService.UnitTests.Common
{
    [TestFixture]
    public class IntegrationTest : SetUpTest
    {
        private IDbContextTransaction _globalTransaction;

        public IServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider { get; private set; }
        public IUnitOfWork UnitOfWork { get; private set; }

        [SetUp]
        public virtual void RunBeforeEachTest()
        {
            Services = InitServiceCollection();
            SetUpServiceCollection(Services);

            ServiceProvider = Services.BuildServiceProvider();
            UnitOfWork = ServiceProvider.GetRequiredService<IUnitOfWork>();

            PrepareTest(ServiceProvider);

            _globalTransaction = ServiceProvider.GetRequiredService<DbContext>().Database.BeginTransaction();
            AddControlValue();
        }

        [SetUp]
        public virtual async Task RunBeforeEachTestAsync()
        {
            await Task.FromResult(default(object));
        }

        [TearDown]
        public virtual void RunAfterEachTest()
        {
            _globalTransaction?.Rollback();

            using (var context = ServiceProvider.GetService<PortfolioReportsServiceDbContext>())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = $"select * from {ControlTable}";
                context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    if (!result.HasRows)
                    {
                        return;
                    }

                    var lastError = string.Empty;
                    while (result.Read())
                    {
                        lastError = result.GetString(0);
                    }

                    throw new TransactionPromotionException(
                        $"There is committed transaction in control table {ControlTable}, last row: {lastError}.");
                }
            }
        }

        private IServiceCollection InitServiceCollection()
        {
            Services = new ServiceCollection();
            Services.AddSingleton(Configuration);
            Services.AddSingleton(CreateLogger());

            Services.AddUnitOfWork<PortfolioReportsServiceDbContext>();

            //Services.AddTaskExecutor();

            Services.AddDbContext<PortfolioReportsServiceDbContext>(options =>
            {
                options.UseSqlServer(Configuration.PortfolioReportsServiceDbContext,
                    config => { });
            });

            Services.AddDbContext<PortfolioReportsServiceDbContext>(options => { options.UseSqlServer(Configuration.PortfolioReportsServiceDbContext); });

            Services.AddApplicationServices();

            return Services;
        }

        protected virtual void PrepareTest(ServiceProvider serviceProvider)
        {
        }

        protected virtual void SetUpServiceCollection(IServiceCollection services)
        {
        }

        protected virtual ILogger CreateLogger()
        {
            SelfLog.Enable(x => Trace.Write(x));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Fatal)
                .WriteTo.Trace()
                .Enrich.WithProperty("Application", "PortfolioReportsServiceIntegrationTest")
                .Enrich.WithProperty("Machine", Environment.MachineName)
                .Filter.With(new DisableLogFilter())
                .CreateLogger();

            return Log.Logger;
        }

        private void AddControlValue()
        {
            ServiceProvider.GetRequiredService<DbContext>().Database
                .ExecuteSqlRaw($"insert into {ControlTable} values (@value)",
                    new SqlParameter("@value", $"{TestContext.CurrentContext.Test.Name} {DateTime.Now}"));
        }
    }
}