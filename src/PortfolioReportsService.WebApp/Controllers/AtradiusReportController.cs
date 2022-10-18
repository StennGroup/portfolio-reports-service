using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortfolioReportsService.Application.Services;

namespace PortfolioReportsService.WebApp.Controllers;

[Route("api/v1/report")]
public class AtradiusReportController: Controller
{
    private readonly AtradiusReportService _reportService;

    public AtradiusReportController(AtradiusReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
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
                    fileStream.Write(reportFiles.Armcust);
            }
            
            return File(memoryStream.ToArray(), "application/zip", $"atradius_report_{DateTime.Today:yyyy_MM_dd}");
        }
    }
}