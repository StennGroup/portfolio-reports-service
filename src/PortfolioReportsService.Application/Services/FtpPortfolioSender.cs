using System.Threading.Tasks;
using Limilabs.FTP.Client;
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
    
    public Task UploadAtradiusReport(AtradiusReports report)
    {
        _logger.Information("Start ftp report upload. Uploading bytes ARMAST: {ArmastFileSize}, ARMCUST: {ArmcustFileSize}",
            report.Armast.Length, report.Armcust.Length);
        using var ftpClient = new Ftp();
        ftpClient.Connect(_config.FtpConfig.Url);
        ftpClient.Login(_config.FtpConfig.Login, _config.FtpConfig.Password);
        ftpClient.Upload("/to_atradius/ARMAST.txt", report.Armast);
        ftpClient.Upload("/to_atradius/ARCUST.txt", report.Armcust);
        _logger.Information("Report files have been uploaded to ftp");
        return Task.CompletedTask;
    }
}