namespace PortfolioReportService.InteropAbstractions.OperationsApi;

public interface IOperationsApi
{
    Task<List<PortfolioInvoiceDto>> GetPortfolioInvoiceInfo();
}