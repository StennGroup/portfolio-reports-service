using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PortfolioReportsService.Persistence
{
    public static class TypeExtensions
    {
        public static bool IsEntityTypeConfiguration(this Type type)
        {
            var entityTypeConfigurationInterface = type
                .GetInterfaces()
                .Where(t => t.IsGenericType)
                .Select(t => new
                {
                    Definition = t.GetGenericTypeDefinition(),
                    Param = t.GetGenericArguments()[0]
                })
                .SingleOrDefault(t => t.Definition == typeof(IEntityTypeConfiguration<>));

            if (entityTypeConfigurationInterface == null)
                return false;
            return true;
        }
    }
}