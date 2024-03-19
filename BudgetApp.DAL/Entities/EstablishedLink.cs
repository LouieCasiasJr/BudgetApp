using System;
using System.Collections.Generic;

namespace BudgetApp.DAL;

public partial class EstablishedLink
{
    public int LinkId { get; set; }

    public string? ContainsText { get; set; }

    public string? Category { get; set; }

    public int BucketId { get; set; }

    public virtual SpendingBucket Bucket { get; set; } = null!;
}
