using System;
using System.Collections.Generic;

namespace BudgetApp.DAL;

public partial class SpendingBucket
{
    public int BucketId { get; set; }

    public string BucketLabel { get; set; } = null!;

    public byte DefaultPriority { get; set; }

    public short DisplayOrder { get; set; }

    public virtual ICollection<EstablishedLink> EstablishedLinks { get; set; } = new List<EstablishedLink>();

    public virtual ICollection<MonthlyBudget> MonthlyBudgets { get; set; } = new List<MonthlyBudget>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
