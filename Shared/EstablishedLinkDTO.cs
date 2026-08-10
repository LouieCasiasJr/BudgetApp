using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Shared;

public partial class EstablishedLinkDTO: BaseDTO
{
    public int? LinkId { get; set; }

    [MaxLength(50)]
    public string? ContainsText { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }

    [Required]
    public int BucketId { get; set; }
}
