using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using PortfolioReportService.InteropAbstractions.OperationsApi.Dto;
using PortfolioReportsService.Application.Interfaces;

namespace PortfolioReportsService.Application.Services;

public class AtradiusReportGenerator : IAtradiusReportGenerator
{
    public byte[] GenerateCustomerReport(IReadOnlyCollection<TradeRelationDto> portfolioData, Dictionary<string, CountryDto> countriesInfo)
    {
        var memory = new MemoryStream();
        using var writer = new StreamWriter(memory);
        
        var csv = new CsvWriter(writer, AtradiusFileConfig.Config);
        
        var builder = new CsvBuilder<TradeRelationDto>();
        builder.AddWithValue("CUSTNO", d => d.Buyer.Duns + d.Seller.SourceSystemId)
            .AddWithValue("COMPANY", d => d.Buyer.Name)
            .AddWithValue("ADDRESS1", d => d.Buyer.BillingStreet)
            .AddWithValue("ADDRESS2", d => d.Buyer.BillingCity + " " + d.Buyer.BillingPostalCode)
            .AddBlank("ADDRESS3")
            .AddBlank("CITY")
            .AddBlank("STATE")
            .AddBlank("ZIP")
            .AddWithValue("COUNTRY", d => countriesInfo.TryGetValue(d.Buyer.Country, out var c) ? c.Name : string.Empty)
            .AddBlank("PHONE")
            .AddBlank("TYPE")
            .AddBlank("TITLE")
            .AddBlank("TERR")
            .AddConst("LOCCURR", "USD")
            .AddBlank("FIRSTNAME")
            .AddBlank("LASTNAME")
            .AddWithValue("BUSINESSUNIT", d => d.Seller.SourceSystemId)
            .AddBlank("EMAIL")
            .AddBlank("SMS_PHONE")
            .AddConst("Debtor_Lang_ID", "EnGB")
            .AddWithValue("FLEXFIELD1", d => d.Seller.Name)
            .AddBlank("FLEXFIELD2")
            .AddBlank("FLEXFIELD3")
            .AddBlank("FLEXFIELD4")
            .AddWithValue("FLEXFIELD5", d => d.Seller.Country);
        for (var i = 6; i <= 34; i++)
            builder.AddBlank($"FLEXFIELD{i}");
        
        var portfolioCustomers = portfolioData.DistinctBy(GetCustomerNumber).ToList();
        
        builder.Write(csv, portfolioCustomers);
        writer.Flush();
        return memory.ToArray();
    }

    private string GetCustomerNumber(TradeRelationDto tradeRelation)
        => tradeRelation.Buyer.Duns + tradeRelation.Seller.SourceSystemId;
}