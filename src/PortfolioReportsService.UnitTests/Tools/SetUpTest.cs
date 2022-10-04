using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Seedwork.Configuration;
using PortfolioReportsService.Infrastructure.Configuration;
using PortfolioReportsService.Persistence.Write;

namespace PortfolioReportsService.UnitTests.Tools
{
    [SetUpFixture]
    public class SetUpTest
    {
        private const string ConfigName = "appsettings.autotests.json";
        protected const string ControlTable = "TestCommits";

        protected PortfolioReportsServiceConfiguration Configuration { get; private set; }

        public SetUpTest()
        {
        }

        [OneTimeSetUp]
        public void RunBeforeAnyTestsOnes()
        {
            Configuration = LoadConfiguration();
            PrepareDb();
        }

        [OneTimeTearDown]
        public void RunAfterAnyTestsOnes()
        {
        }

        private static PortfolioReportsServiceConfiguration LoadConfiguration()
        {
            var appConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: ConfigName, optional: false, reloadOnChange: true)
                .Build();
            var settingsConnectionString = appConfig.GetValue<string>("SettingsStorageConnectionString");
            var defaultSettings = appConfig.GetValue<string>("SettingsName");
            var loadConfigLocal = appConfig.GetValue<bool>("LoadConfigLocal");
            var buildAgent = appConfig.GetValue<string>("BuildAgent");
            var reader = new ConfigurationReader(settingsConnectionString, defaultSettings, loadConfigLocal);
            var configuration = reader.ReadConfiguration<PortfolioReportsServiceConfiguration>();
            if (!string.IsNullOrWhiteSpace(buildAgent))
            {
                var dbNameMatch = Regex.Match(configuration.PortfolioReportsServiceDbContext, @"(?<=Initial Catalog=)\S+Test");
                if (dbNameMatch.Success)
                    configuration.PortfolioReportsServiceDbContext =
                        configuration.PortfolioReportsServiceDbContext.Replace(dbNameMatch.Value,
                            $"{dbNameMatch.Value}-{buildAgent}");
            }

            return configuration;
        }

        private void PrepareDb()
        {
            var optionsBuilder = new DbContextOptionsBuilder<PortfolioReportsServiceDbContext>();
            optionsBuilder
                .UseSqlServer(Configuration.PortfolioReportsServiceDbContext);

            using (var context = new PortfolioReportsServiceDbContext(optionsBuilder.Options))
            {
                if (!context.Database.GetDbConnection().ConnectionString.ToUpper().Contains("SQLEXPRESS"))
                {
                    throw new InvalidOperationException("Trying to operate with non local database.");
                }

                var recreateDb = false;
                var createControlTableCommand =
                    $"if not exists (select * from sysobjects where name='{ControlTable}' and xtype='U') " +
                    $"create table {ControlTable} (ErrorTest varchar(max) not null)";

                if (((RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>()).Exists())
                {
                    context.Database.ExecuteSqlRaw(createControlTableCommand);

                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = $"select * from {ControlTable}";
                        context.Database.OpenConnection();
                        using (var result = command.ExecuteReader())
                        {
                            if (result.HasRows)
                            {
                                recreateDb = true;
                            }
                        }
                    }
                }
                else
                {
                    recreateDb = true;
                }

                if (recreateDb)
                {
                    context.Database.EnsureDeleted();
                }

                context.Database.Migrate();
                context.Database.ExecuteSqlRaw(createControlTableCommand);
            }
        }
    }
}