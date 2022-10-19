using PortfolioReportService.InteropAbstractions.OperationsApi;
using PortfolioReportService.InteropAbstractions.OperationsApi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PortfolioReportsService.Infrastructure.OperationsApi;

internal class ODataResponse<T> where T: class, new()
{
    public T Value { get; set; }
}

public class OperationsApi : IOperationsApi
{
    private readonly HttpClient _httpClient;

    public OperationsApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<PortfolioInvoiceDto>> GetPortfolioInvoiceInfo()
    {
        var response = await _httpClient.GetFromJsonAsync<ODataResponse<List<PortfolioInvoiceDto>>>("api/odata/v1/invoices?" +
            "$filter=repaymentAmountOutstanding/amount gt 0 and owner eq 'SDF'&" +
            "$select=id,repaymentAmountOutstanding,repaymentAmount,supplyDate,dueDate,repaymentAmountNationalCurrency,repaymentAmountOutstandingNationalCurrency&" +
            "$expand=tradeRelation($select=id;$expand=buyer($select=duns,name,sourceSystemId,billingStreet,billingCity,billingPostalCode, country)," +
                "seller($select=duns,name,sourceSystemId,billingStreet,billingCity,billingPostalCode, country))");

        return response?.Value ?? new List<PortfolioInvoiceDto>();
    }

    public async Task<List<CountryDto>> GetCountriesInfo()
        => await _httpClient.GetFromJsonAsync<List<CountryDto>>("api/v1/countries/all") ?? new List<CountryDto>();
    
}