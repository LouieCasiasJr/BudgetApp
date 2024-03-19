using System;
using System.Collections.Generic;

namespace BudgetApp.DAL;

public partial class MonthlyBudget
{
    public int MonthlyBudgetId { get; set; }

    public int BucketId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal Amount { get; set; }

    public virtual SpendingBucket Bucket { get; set; } = null!;
}
