using System.Threading.Tasks;
using FluentFTP;
using PortfolioReportsService.Application.Interfaces;
using PortfolioReportsService.Infrastructure.Configuration;
using Serilog;

namespace PortfolioReportsService.Application.Services;

class FtpPortfolioSender : IPortfolioSender
{
    private readonly PortfolioReportsServiceConfiguration _config;
    private readonly ILogger _logger;

    public FtpPortfolioSender(PortfolioReportsServiceConfiguration config, ILogger logger)
    {
        _config = config;
        _logger = logger;
    }
    
    public async Task UploadAtradiusReport(AtradiusReports report)
    {
        _logger.Information("Start ftp report upload. Uploading bytes ARMAST: {ArmastFileSize}, ARMCUST: {ArmcustFileSize}",
            report.Armast.Length, report.Armcust.Length);
        using var ftpClient = new AsyncFtpClient(_config.FtpConfig.Url, _config.FtpConfig.Login, _config.FtpConfig.Password);
        await ftpClient.AutoConnect();
        await ftpClient.UploadBytes(report.Armast, "/to_atradius/ARMAST.txt");
        await ftpClient.UploadBytes(report.Armcust, "/to_atradius/ARCUST.txt");
        await ftpClient.Disconnect();
        _logger.Information("Report files have been uploaded to ftp");
    }
}