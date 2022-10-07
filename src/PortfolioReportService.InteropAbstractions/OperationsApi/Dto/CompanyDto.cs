namespace PortfolioReportService.InteropAbstractions.OperationsApi.Dto;

public class CompanyDto
{
    public string? Duns { get; set; }
    public string Name { get; set; } = null!;
    public string SourceSystemId { get; set; } = null!;
    public string BillingStreet { get; set; } = null!;
    public string BillingCity { get; set; } = null!;
    public string? BillingPostalCode { get; set; }
    public string Country { get; set; } = null!;
}
