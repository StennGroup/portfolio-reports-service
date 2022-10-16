using System.Linq;
using System.Threading.Tasks;
using PortfolioReportService.InteropAbstractions.OperationsApi;
using PortfolioReportsService.Application.Interfaces;

namespace PortfolioReportsService.Application.Services;

public class AtradiusReports
{
    public byte[] Armast { get; set; }
    public byte[] Armcust { get; set; }
}

public class AtradiusReportService
{
    private readonly IOperationsApi _operationsApi;
    private readonly IAtradiusReportGenerator _reportGenerator;

    public AtradiusReportService(IOperationsApi operationsApi, IAtradiusReportGenerator reportGenerator)
    {
        _operationsApi = operationsApi;
        _reportGenerator = reportGenerator;
    }

    public async Task<AtradiusReports> SendReport()
    {
        var portfolio = await _operationsApi.GetPortfolioInvoiceInfo();
        var countries = await _operationsApi.GetCountriesInfo();
        
        var countryCodeLookup = countries.ToDictionary(c => c.Code);
        var customerPairs = portfolio.Select(i => i.TradeRelation).ToList();
        var customersFile = _reportGenerator.GenerateCustomerReport(customerPairs, countryCodeLookup);
        
        var invoicesFile = _reportGenerator.GenerateInvoicesReport(portfolio);

        return new AtradiusReports
        {
            Armast = invoicesFile,
            Armcust = customersFile
        };
    }
}