using System.Threading.Tasks;
using PortfolioReportsService.Application.Services;

namespace PortfolioReportsService.Application.Interfaces;

public interface IAtradiusReportService
{
    Task<AtradiusReports> GenerateReport();
}