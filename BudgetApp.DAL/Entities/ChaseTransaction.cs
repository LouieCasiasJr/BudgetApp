using System;
using System.Collections.Generic;

namespace BudgetApp.DAL;

public partial class ChaseTransaction
{
    public int ChaseTransactionId { get; set; }

    public string Details { get; set; } = null!;

    public DateOnly PostingDate { get; set; }

    public string Description { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Type { get; set; } = null!;

    public string? RefNumber { get; set; }
}
