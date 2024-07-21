namespace BudgetApp.Shared;

public partial class SpendingBucketResultDTO: BaseDTO
{
    public string BucketLabel { get; set; } = null!;
    public byte DefaultPriority { get; set; }
    public decimal Delta { get; set; }
    public decimal Budget { get; set; }
}
