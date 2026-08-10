namespace BudgetApp.Shared;

public class ChangeResultPacket<T> where T : new()
{
    public ChangeResultPacket()
    {
        SucccessSet = new List<T>();
        FailureSet = new List<T>();
    }
    public string? ErrorMessage { get; set; }
    public int? SuccessCount { get; set; }
    public int? FailureCount { get; set; }
    public List<T>? SucccessSet { get; set; }
    public List<T>? FailureSet { get; set; }
}