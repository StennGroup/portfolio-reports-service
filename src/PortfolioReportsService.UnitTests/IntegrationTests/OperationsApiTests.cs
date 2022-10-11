using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using PortfolioReportsService.Infrastructure.OperationsApi;

namespace PortfolioReportsService.UnitTests.IntegrationTests;

[TestFixture]
[Explicit("Run only to ensure contract with your eyes")]
public class OperationsApiTests
{
    [Test]
    public async Task TestPortfolioItemsLoading()
    {
        using var client = new HttpClient()
        {
            BaseAddress = new Uri("https://localhost:44306/")
        };
        var opsClient = new OperationsApi(client);

        var portfolio = await opsClient.GetPortfolioInvoiceInfo();
        Console.WriteLine(JsonSerializer.Serialize(portfolio, new JsonSerializerOptions {WriteIndented = true}));
    }
}