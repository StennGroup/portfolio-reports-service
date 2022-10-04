using System;
using Microsoft.Extensions.Configuration;

namespace PortfolioReportsService.Infrastructure
{
    public static class EnvironmentChecker
    {
        private static string LocalDbName = "SQLEXPRESS";
        public static string AllowDbMigrationVar = "AllowDbMigration";
        public static string AllowDbMigrationValue = "TRUE";

        public static void CheckTheSafetyOfMigrations(string connectionString, IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(connectionString))
                return;

            var isLocalDb = connectionString.ToUpper().Contains(LocalDbName);

            if (isLocalDb)
                return;

            if (configuration == null)
                throw new ArgumentException("Configuration param is not set");

            var isAllowDbMigration = configuration.GetValue<string>(AllowDbMigrationVar);

            if (isAllowDbMigration == null || isAllowDbMigration.ToUpper() != AllowDbMigrationValue)
                throw new ApplicationException(
                    "Trying to operate with non local database in development mode or parameter AllowDbMigration is not configured.");
        }
    }
}