namespace BudgetApp.Shared;

public partial class MonthlyBudgetDTO: BaseDTO
{
    public int MonthlyBudgetId { get; set; }

    public int BucketId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal Amount { get; set; }
}
