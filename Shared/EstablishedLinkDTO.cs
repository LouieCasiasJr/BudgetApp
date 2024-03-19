namespace BudgetApp.Shared;

public partial class EstablishedLinkDTO: BaseDTO
{
    public int LinkId { get; set; }

    public string? ContainsText { get; set; }

    public string? Category { get; set; }

    public int BucketId { get; set; }
}
