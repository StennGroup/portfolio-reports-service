namespace PortfolioReportService.InteropAbstractions.OperationsApi.Dto;

public class PortfolioInvoiceDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = null!;
    public DateTime SupplyDate { get; set; }
    public DateTime DueDate { get; set; }
    public MoneyDto RepaymentAmountOutstanding { get; set; } = null!;
    public MoneyDto RepaymentAmount { get; set; } = null!;
    public MoneyDto RepaymentAmountOutstandingNationalCurrency { get; set; } = null!;
    public MoneyDto RepaymentAmountNationalCurrency { get; set; } = null!;
    public TradeRelationDto TradeRelation { get; set; } = null!;

}
