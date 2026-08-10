using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Shared;

public partial class TransactionDTO : BaseDTO
{
    public int TransactionId { get; set; }

    [Required]
    public DateOnly TransactionDate { get; set; }

    [Required]
    [MaxLength(8)]
    public string Card { get; set; } = null!;

    [Required]
    [MaxLength(150)]
    public string Description { get; set; } = null!;

    [Required]
    public bool Debit { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }

    [MaxLength(25)]
    public string? Reference { get; set; }

    public int? BucketId { get; set; }

    public byte Priority { get; set; }

    [Required]
    [MaxLength(25)]
    public string Currency { get; set; }

    public string? Message { get; set; }
}
