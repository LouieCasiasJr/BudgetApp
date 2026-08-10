using System;
using System.Collections.Generic;

namespace BudgetApp.DAL;

public partial class Transaction
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

    public byte? Priority { get; set; }

    public string Currency { get; set; } = null!;

    public virtual SpendingBucket? Bucket { get; set; }
}
