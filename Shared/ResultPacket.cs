namespace BudgetApp.Shared
{
    public class ResultPacket<T> where T : new()
    {
        public ResultPacket() {
            Data = new T();
        }

        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
