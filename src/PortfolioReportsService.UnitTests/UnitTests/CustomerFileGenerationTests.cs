using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using FluentAssertions;
using NUnit.Framework;
using PortfolioReportService.InteropAbstractions.OperationsApi.Dto;
using PortfolioReportsService.Application;
using PortfolioReportsService.Application.Services;

namespace PortfolioReportsService.UnitTests.UnitTests;


[TestFixture]
public class CustomerFileGenerationTests
{
    private readonly Dictionary<string, CountryDto> _countries = new()
    {
        { "CL", new CountryDto { Name = "Chile", Code = "CL"} },
        { "CN", new CountryDto { Name = "China", Code = "CN"} },
        { "SK", new CountryDto { Name = "Slovakia", Code = "SK"} }
    };

    [Test]
    public void GenerateCustomerReport_WithOneCompanyPair_CreatesCorrectFile()
    {
        var testData = new[]
        {
            TradeRelationFixture(tr =>
            {
                tr.Buyer.Duns = "1";
                tr.Seller.SourceSystemId = "seller-1";
            }),
        };

        var fileGenerator = new AtradiusReportGenerator();

        var file = fileGenerator.GenerateCustomerReport(testData, _countries);

        var fileRows = ReadAtradiusFileToDictionary(file);

        fileRows.Should().HaveCount(1);
        var rowToCheck = fileRows.First();

        rowToCheck["CUSTNO"].Should().Be("1seller-1");
        rowToCheck["COUNTRY"].Should().Be("Chile");
    }

    [Test]
    public void GenerateCustomerReport_WhenTradeRelationsWithSameBuyerAndSeller_ShouldTakeOne()
    {
        var testData = new[]
        {
            TradeRelationFixture(tr =>
            {
                tr.Buyer.Duns = "1";
                tr.Seller.SourceSystemId = "seller-1";
            }),
            TradeRelationFixture(tr =>
            {
                tr.Buyer.Duns = "1";
                tr.Seller.SourceSystemId = "seller-1";
            })
        };

        var fileGenerator = new AtradiusReportGenerator();

        var file = fileGenerator.GenerateCustomerReport(testData, _countries);

        var fileRows = ReadAtradiusFileToDictionary(file);

        fileRows.Should().HaveCount(1);
        var rowToCheck = fileRows.First();

        rowToCheck["CUSTNO"].Should().Be("1seller-1");
        rowToCheck["COUNTRY"].Should().Be("Chile");
    }
    
    [Test]
    public void GenerateCustomerReport_WithTwoDifferentCustomerPairs_CreatesTwoRows()
    {
        var testData = new[]
        {
            TradeRelationFixture(tr =>
            {
                tr.Buyer.Duns = "1";
                tr.Seller.SourceSystemId = "seller-1";
            }),
            TradeRelationFixture(tr =>
            {
                tr.Buyer.Duns = "2";
                tr.Seller.SourceSystemId = "seller-2";
                tr.Buyer.Country = "CN";
            })
        };

        var fileGenerator = new AtradiusReportGenerator();

        var file = fileGenerator.GenerateCustomerReport(testData, _countries);

        var fileRows = ReadAtradiusFileToDictionary(file);

        fileRows.Should().HaveCount(2);
        var firstRow = fileRows.First();
        var secondRow = fileRows.ElementAt(1);

        firstRow["CUSTNO"].Should().Be("1seller-1");
        firstRow["COUNTRY"].Should().Be("Chile");
        secondRow["CUSTNO"].Should().Be("2seller-2");
        secondRow["COUNTRY"].Should().Be("China");
    }

    private IReadOnlyCollection<Dictionary<string, string>> ReadAtradiusFileToDictionary(byte[] file)
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

    private TradeRelationDto TradeRelationFixture(Action<TradeRelationDto> change)
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
        change(defaultTradeRelation);
        return defaultTradeRelation;
    }
}