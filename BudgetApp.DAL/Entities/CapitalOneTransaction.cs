using System;
using System.Collections.Generic;

namespace BudgetApp.DAL;

public partial class CapitalOneTransaction
{
    public int CapitalOneTransactionId { get; set; }

    public DateOnly TransactionDate { get; set; }

    public string Card { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Category { get; set; } = null!;

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }
}
