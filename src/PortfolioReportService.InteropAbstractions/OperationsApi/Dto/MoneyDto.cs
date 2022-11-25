namespace PortfolioReportService.InteropAbstractions.OperationsApi.Dto;

public class MoneyDto
{
    public decimal Amount { get; set; }
    public CurrrencyDto Currency { get; set; } = null!;
}
