using PortfolioReportService.InteropAbstractions.OperationsApi.Dto;

namespace PortfolioReportService.InteropAbstractions.OperationsApi;

public interface IOperationsApi
{
    Task<List<PortfolioInvoiceDto>> GetPortfolioInvoiceInfo();
}