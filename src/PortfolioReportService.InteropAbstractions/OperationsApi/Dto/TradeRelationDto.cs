namespace PortfolioReportService.InteropAbstractions.OperationsApi.Dto;

public class TradeRelationDto
{
    public CompanyDto Buyer { get; set; } = null!;
    public CompanyDto Seller { get; set; } = null!;
}
