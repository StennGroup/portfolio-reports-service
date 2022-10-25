using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortfolioReportsService.Application.Interfaces;
using Seedwork.Web;

namespace PortfolioReportsService.WebApp.Controllers;

[Route("api/v1/report")]
public class AtradiusReportController : Controller
{
    private readonly IAtradiusReportService _reportService;
    private readonly LongTaskWebExecutor _taskExecutor;
    private readonly IPortfolioSender _portfolioSender;

    public AtradiusReportController(IAtradiusReportService reportService, LongTaskWebExecutor taskExecutor, IPortfolioSender portfolioSender)
    {
        _reportService = reportService;
        _taskExecutor = taskExecutor;
        _portfolioSender = portfolioSender;
    }

    [HttpGet("send-report")]
    public async Task SendAtradiusReport()
    {
        await _taskExecutor.Execute(SendAtradiusReportInternal(), Response);
    }

    [HttpGet("get-report")]
    public async Task<FileResult> GetAtradiusReport()
    {
        var reportFiles = await _reportService.GenerateReport();
        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create))
            {
                var arcustEntry = archive.CreateEntry("ARCUST.txt");
                using (var fileStream = arcustEntry.Open())
                    fileStream.Write(reportFiles.Armcust);

                var armastEntry = archive.CreateEntry("ARMAST.txt");
                using (var fileStream = armastEntry.Open())
                    fileStream.Write(reportFiles.Armast);
            }

            return File(memoryStream.ToArray(), "application/zip", $"atradius_report_{DateTime.Today:yyyy_MM_dd}.zip");
        }
    }
    
    private async Task SendAtradiusReportInternal()
    {
        var reportFiles = await _reportService.GenerateReport();
        await _portfolioSender.UploadAtradiusReport(reportFiles);
    }
}