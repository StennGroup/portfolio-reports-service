using PortfolioReportService.InteropAbstractions.OperationsApi;
using PortfolioReportService.InteropAbstractions.OperationsApi.Dto;
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
    private readonly HttpClient _httpClient;

    public OperationsApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<List<PortfolioInvoiceDto>> GetPortfolioInvoiceInfo()
        => _httpClient.GetFromJsonAsync<List<PortfolioInvoiceDto>>("api/odata/v1/invoices?" +
            "$filter=repaymentAmountOutstanding/amount gt 0 and owner eq 'SDF'&" +
            "$select=id,repaymentAmountOutstanding,repaymentAmount,supplyDate,dueDate,repaymentAmountNationalCurrency,repaymentAmountOutstandingNationalCurrency&" +
            "$expand=tradeRelation($select=id;$expand=buyer($select=duns,name,sourceSystemId,billingStreet,billingCity,billingPostalCode, country)," +
                "seller($select=duns,name,sourceSystemId,billingStreet,billingCity,billingPostalCode, country))");
}