using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using PortfolioReportService.InteropAbstractions.OperationsApi.Dto;
using PortfolioReportsService.Application.Services;
using PortfolioReportsService.UnitTests.Common;

namespace PortfolioReportsService.UnitTests.UnitTests;

[TestFixture]
public class InvoiceFileGenerationTest
{
    [Test]
    public void TestInvoiceWritesCorrectly()
    {
        //Arrange
        var invoices = new PortfolioInvoiceDto[]
        {
            AtradiusReportTestUtils.InvoiceFixture(i =>
            {
                i.SupplyDate = new DateTime(2022, 10, 12);
                i.DueDate = i.SupplyDate.AddDays(10);

                i.RepaymentAmountNationalCurrency.Currency.Iso3LetterCode = "JPY";
                i.RepaymentAmountNationalCurrency.Amount = 100.53m;
                
                i.TradeRelation.Buyer.Duns = "1";
                i.TradeRelation.Seller.SourceSystemId = "seller-1";
            })
        };
        
        //Act
        var file = new AtradiusReportGenerator().GenerateInvoicesReport(invoices);
        
        //Assert
        var fileRows =  AtradiusReportTestUtils.ReadAtradiusFileToDictionary(file);

        fileRows.Should().HaveCount(1);
        var rowToCheck = fileRows.First();

        rowToCheck["CUSTNO"].Should().Be("1seller-1");
        rowToCheck["PNET"].Should().Be("10");
        rowToCheck["TRANCURR"].Should().Be("JPY");
        rowToCheck["TRANORIG"].Should().Be("100.53");
    }
}