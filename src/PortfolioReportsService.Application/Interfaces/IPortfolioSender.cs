using System.Threading.Tasks;
using PortfolioReportsService.Application.Services;

namespace PortfolioReportsService.Application.Interfaces;

public interface IPortfolioSender
{
    Task UploadAtradiusReport(AtradiusReports report);
}