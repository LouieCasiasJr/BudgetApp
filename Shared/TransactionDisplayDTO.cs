namespace BudgetApp.Shared;

public partial class TransactionDisplayDTO : BaseDTO
{
    public DateOnly TransactionDate { get; set; }

    public string Card { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Debit { get; set; }

    public decimal Amount { get; set; }

    public string? Reference { get; set; }

    public string BucketLabel { get; set; } = null!;

    public byte? Priority { get; set; }
}
