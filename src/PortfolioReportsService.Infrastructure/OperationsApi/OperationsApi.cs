using PortfolioReportService.InteropAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioReportsService.Infrastructure.OperationsApi;

internal class OperationsApi : IOperationsApi
{
    private readonly HttpClient httpClient;

    public OperationsApi(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public Task<List<PortfolioInvoiceDto>> GetPortfolioInvoiceInfo()
        => httpClient.GetFromJsonAsync<List<PortfolioInvoiceDto>>("api/odata/v1/invoices?" +
            "$filter=repaymentAmountOutstanding/amount gt 0 and owner eq 'SDF'&" +
            "$select=id,repaymentAmountOutstanding,repaymentAmount,supplyDate,dueDate,repaymentAmountNationalCurrency,repaymentAmountOutstandingNationalCurrency&" +
            "$expand=tradeRelation($select=id;$expand=buyer($select=duns,name,sourceSystemId,billingStreet,billingCity,billingPostalCode, country)," +
                "seller($select=duns,name,sourceSystemId,billingStreet,billingCity,billingPostalCode, country))");
}