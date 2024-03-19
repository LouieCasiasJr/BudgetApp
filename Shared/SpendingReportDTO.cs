namespace BudgetApp.Shared;

public partial class SpendingReportResponse : BaseDTO
{
    public bool isSuccess { get; set; }
    public List<SpendingReportDTO> SpendingReports { get; set; }
    public string message { get; set; }
}

public partial class SpendingReportDTO: BaseDTO
{
    public string Period { get; set; }

    public List<SpendingBucketResultDTO> Deltas { get; set; }

    public Dictionary<string, decimal> CardTotals { get; set; }

    public decimal Income { get; set; }
    public decimal Budgeted { get; set; }

    public List<TransactionDisplayDTO>? SummaryDisplay { get; set; }
}
