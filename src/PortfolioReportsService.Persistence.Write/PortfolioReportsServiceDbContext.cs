using Microsoft.EntityFrameworkCore;
using PortfolioReportsService.Persistence.Write.Mappings.Internals;

namespace PortfolioReportsService.Persistence.Write
{
    public class PortfolioReportsServiceDbContext : DbContext
    {
        public PortfolioReportsServiceDbContext(DbContextOptions<PortfolioReportsServiceDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SafeAssemblySearchAncestor).Assembly,
                t => t.IsEntityTypeConfiguration());
        }
    }
}