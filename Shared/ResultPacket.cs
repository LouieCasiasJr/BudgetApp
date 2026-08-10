namespace BudgetApp.Shared
{
    public class ResultPacket<T> where T : new()
    {
        public ResultPacket() {
            Data = new T();
        }

        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string? Message { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPageCount { get; set; }
        public bool? HasPreviousPage { get; set; }
        public bool? HasNextPage { get; set; }
    }
}
