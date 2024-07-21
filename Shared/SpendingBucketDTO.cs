namespace BudgetApp.Shared;

public partial class SpendingBucketDTO: BaseDTO
{
    public int BucketId { get; set; }

    public string BucketLabel { get; set; } = null!;

    public byte DefaultPriority { get; set; }
}
