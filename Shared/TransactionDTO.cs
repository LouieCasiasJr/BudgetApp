namespace BudgetApp.Shared;

public partial class TransactionDTO : BaseDTO
{
    public int TransactionId { get; set; }

    public DateOnly TransactionDate { get; set; }

    public string Card { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Debit { get; set; }

    public decimal Amount { get; set; }

    public string? Category { get; set; }

    public string? Reference { get; set; }

    public int? BucketId { get; set; }

    public byte Priority { get; set; }
}
