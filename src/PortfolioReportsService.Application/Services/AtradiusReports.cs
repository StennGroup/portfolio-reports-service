using System;

namespace PortfolioReportsService.Application.Services;

public class AtradiusReports
{
    public DateTime Date { get; set; }
    public byte[] Armast { get; set; } = null!;
    public byte[] Arcust { get; set; } = null!;
}