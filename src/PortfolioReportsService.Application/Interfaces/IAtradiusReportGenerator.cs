using System.Collections.Generic;
using PortfolioReportService.InteropAbstractions.OperationsApi.Dto;

namespace PortfolioReportsService.Application.Interfaces;

public interface IAtradiusReportGenerator
{
    byte[] GenerateCustomerReport(IReadOnlyCollection<TradeRelationDto> portfolioData, Dictionary<string, CountryDto> countriesInfo);
    byte[] GenerateInvoicesReport(IReadOnlyCollection<PortfolioInvoiceDto> portfolioData);
}