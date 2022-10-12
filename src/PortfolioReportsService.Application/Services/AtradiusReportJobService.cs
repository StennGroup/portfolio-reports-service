using System.Linq;
using System.Threading.Tasks;
using PortfolioReportService.InteropAbstractions.OperationsApi;
using PortfolioReportsService.Application.Interfaces;

namespace PortfolioReportsService.Application.Services;

public class AtradiusReportJobService
{
    private readonly IOperationsApi _operationsApi;
    private readonly IAtradiusReportGenerator _reportGenerator;

    public AtradiusReportJobService(IOperationsApi operationsApi, IAtradiusReportGenerator reportGenerator)
    {
        _operationsApi = operationsApi;
        _reportGenerator = reportGenerator;
    }

    public async Task SendReport()
    {
        var portfolio = await _operationsApi.GetPortfolioInvoiceInfo();
        var countries = await _operationsApi.GetCountriesInfo();
        
        var countryCodeLookup = countries.ToDictionary(c => c.Code);
        var customerPairs = portfolio.Select(i => i.TradeRelation).ToList();
        var customersFile = _reportGenerator.GenerateCustomerReport(customerPairs, countryCodeLookup);
        
        var invoicesFile = _reportGenerator.GenerateInvoicesReport(portfolio);

        //todo add armast file generation and ftp sending
    }
}