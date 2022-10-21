using System.Linq;
using System.Threading.Tasks;
using PortfolioReportService.InteropAbstractions.OperationsApi;
using PortfolioReportsService.Application.Interfaces;
using Serilog;

namespace PortfolioReportsService.Application.Services;

public class AtradiusReports
{
    public byte[] Armast { get; set; }
    public byte[] Armcust { get; set; }
}

public class AtradiusReportService : IAtradiusReportService
{
    private readonly IOperationsApi _operationsApi;
    private readonly IAtradiusReportGenerator _reportGenerator;
    private readonly ILogger _logger;

    public AtradiusReportService(IOperationsApi operationsApi, IAtradiusReportGenerator reportGenerator, ILogger logger)
    {
        _operationsApi = operationsApi;
        _reportGenerator = reportGenerator;
        _logger = logger;
    }

    public async Task<AtradiusReports> GenerateReport()
    {
        _logger.Debug("Start report generation");
        var portfolio = await _operationsApi.GetPortfolioInvoiceInfo();
        var countries = await _operationsApi.GetCountriesInfo();
        _logger.Debug("Invoices and countries data received. Total invoices to report {InvoicesCount}", portfolio.Count);
        
        var countryCodeLookup = countries.ToDictionary(c => c.Code);
        var customerPairs = portfolio.Select(i => i.TradeRelation).ToList();
        var customersFile = _reportGenerator.GenerateCustomerReport(customerPairs, countryCodeLookup);
        _logger.Debug("Customer file generated successfully with {CustomersFileSize} bytes", customersFile.Length);
        
        var invoicesFile = _reportGenerator.GenerateInvoicesReport(portfolio);
        _logger.Debug("Invoices file generated successfully with {InvoicesFileSize} bytes", invoicesFile.Length);
        
        return new AtradiusReports
        {
            Armast = invoicesFile,
            Armcust = customersFile
        };
    }
}