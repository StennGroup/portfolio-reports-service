using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using PortfolioReportService.InteropAbstractions.OperationsApi.Dto;
using PortfolioReportsService.Application.Services;
using PortfolioReportsService.UnitTests.Common;

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
        //Assert
        var testData = new[]
        {
            AtradiusReportTestUtils.TradeRelationFixture(tr =>
            {
                tr.Buyer.Duns = "1";
                tr.Seller.SourceSystemId = "seller-1";
            }),
        };
        var fileGenerator = new AtradiusReportGenerator();
        
        //Act
        var file = fileGenerator.GenerateCustomerReport(testData, _countries);
        
        //Assert
        var fileRows =  AtradiusReportTestUtils.ReadAtradiusFileToDictionary(file);

        fileRows.Should().HaveCount(1);
        var rowToCheck = fileRows.First();

        rowToCheck["CUSTNO"].Should().Be("1seller-1");
        rowToCheck["COUNTRY"].Should().Be("Chile");
    }

    [Test]
    public void GenerateCustomerReport_WhenTradeRelationsWithSameBuyerAndSeller_ShouldTakeOne()
    {
        //Arrange
        var testData = new[]
        {
            AtradiusReportTestUtils.TradeRelationFixture(tr =>
            {
                tr.Buyer.Duns = "1";
                tr.Seller.SourceSystemId = "seller-1";
            }),
            AtradiusReportTestUtils.TradeRelationFixture(tr =>
            {
                tr.Buyer.Duns = "1";
                tr.Seller.SourceSystemId = "seller-1";
            })
        };

        var fileGenerator = new AtradiusReportGenerator();
        //Act
        var file = fileGenerator.GenerateCustomerReport(testData, _countries);
        
        //Assert
        var fileRows =  AtradiusReportTestUtils.ReadAtradiusFileToDictionary(file);

        fileRows.Should().HaveCount(1);
        var rowToCheck = fileRows.First();

        rowToCheck["CUSTNO"].Should().Be("1seller-1");
        rowToCheck["COUNTRY"].Should().Be("Chile");
    }
    
    [Test]
    public void GenerateCustomerReport_WithTwoDifferentCustomerPairs_CreatesTwoRows()
    {
        //Arrange
        var testData = new[]
        {
            AtradiusReportTestUtils.TradeRelationFixture(tr =>
            {
                tr.Buyer.Duns = "1";
                tr.Seller.SourceSystemId = "seller-1";
                tr.Buyer.Country = "CL";
            }),
            AtradiusReportTestUtils.TradeRelationFixture(tr =>
            {
                tr.Buyer.Duns = "2";
                tr.Seller.SourceSystemId = "seller-2";
                tr.Buyer.Country = "CN";
            })
        };
        var fileGenerator = new AtradiusReportGenerator();
        
        //Act
        var file = fileGenerator.GenerateCustomerReport(testData, _countries);

        //Assert
        var fileRows = AtradiusReportTestUtils.ReadAtradiusFileToDictionary(file);

        fileRows.Should().HaveCount(2);
        var firstRow = fileRows.First();
        var secondRow = fileRows.ElementAt(1);

        firstRow["CUSTNO"].Should().Be("1seller-1");
        firstRow["COUNTRY"].Should().Be("Chile");
        
        secondRow["CUSTNO"].Should().Be("2seller-2");
        secondRow["COUNTRY"].Should().Be("China");
    }
}