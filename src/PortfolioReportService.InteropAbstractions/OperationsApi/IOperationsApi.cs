using PortfolioReportService.InteropAbstractions.OperationsApi.Dto;

namespace PortfolioReportService.InteropAbstractions.OperationsApi;

public interface IOperationsApi
{
    Task<IReadOnlyCollection<PortfolioInvoiceDto>> GetPortfolioInvoiceInfo();
    Task<List<CountryDto>> GetCountriesInfo();
}