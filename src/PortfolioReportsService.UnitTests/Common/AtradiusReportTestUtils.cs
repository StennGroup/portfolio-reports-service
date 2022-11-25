using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using PortfolioReportService.InteropAbstractions.OperationsApi.Dto;
using PortfolioReportsService.Application.Services;

namespace PortfolioReportsService.UnitTests.Common;

public static class AtradiusReportTestUtils
{
    public static IReadOnlyCollection<Dictionary<string, string>> ReadAtradiusFileToDictionary(byte[] file)
    {
        var test = AtradiusFileConfig.Utf8WithoutBom.GetString(file);
        var result = new List<Dictionary<string, string>>();
        using var csvReader = new CsvReader(new StreamReader(new MemoryStream(file)), AtradiusFileConfig.Config);
        csvReader.Read();
        csvReader.ReadHeader();

        while (csvReader.Read())
        {
            var row = new Dictionary<string, string>();
            foreach (var fieldName in csvReader.HeaderRecord)
            {
                row.Add(fieldName, csvReader.GetField(fieldName));
            }

            result.Add(row);
        }

        return result;
    }

    public static PortfolioInvoiceDto InvoiceFixture(Action<PortfolioInvoiceDto>? change = null)
    {
        var defaultInvoice = new PortfolioInvoiceDto
        {
            DueDate = new DateTime(2022, 10, 20),
            SupplyDate = new DateTime(2022, 10, 11),
            Id = Guid.NewGuid(),
            Number = "default-test-123",
            RepaymentAmount = new MoneyDto { Amount = 1000.92m, Currency = new CurrrencyDto {Iso3LetterCode = "USD", IsoNumericCode = 840}},
            RepaymentAmountNationalCurrency = new MoneyDto { Amount = 1000.92m, Currency = new CurrrencyDto {Iso3LetterCode = "USD", IsoNumericCode = 840}},
            RepaymentAmountOutstanding = new MoneyDto { Amount = 100.12m, Currency = new CurrrencyDto {Iso3LetterCode = "USD", IsoNumericCode = 840}},
            RepaymentAmountOutstandingNationalCurrency= new MoneyDto { Amount = 100.12m, Currency = new CurrrencyDto {Iso3LetterCode = "USD", IsoNumericCode = 840}},
            TradeRelation = TradeRelationFixture()
        };
        change?.Invoke(defaultInvoice);
        return defaultInvoice;
    }
    
    public static TradeRelationDto TradeRelationFixture(Action<TradeRelationDto>? change = null)
    {
        var defaultTradeRelation = new TradeRelationDto
        {
            Buyer = new CompanyDto
            {
                BillingCity = "SANTIAGO",
                BillingStreet = "Avda Fermin Viviceta 1258 Santiago",
                BillingPostalCode = null,
                SourceSystemId = "buyer-1",
                Country = "CL",
                Duns = "1",
                Name = "Test-1"
            },
            Seller = new CompanyDto
            {
                BillingCity = "HALFWAY HOUSE",
                BillingStreet = "89 SAHARA AV CORPORATE OFFICE PARK OLD PTA RD",
                BillingPostalCode = "1683",
                SourceSystemId = "seller-1",
                Country = "ZA",
                Duns = "1",
                Name = "Test-1"
            },
        };
        change?.Invoke(defaultTradeRelation);
        return defaultTradeRelation;
    }
}