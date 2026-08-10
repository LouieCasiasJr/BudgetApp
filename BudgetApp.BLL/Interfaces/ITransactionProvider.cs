using BudgetApp.Shared;

namespace BudgetApp.BLL
{
    public interface ITransactionProvider
    {
        public IEnumerable<TransactionDTO> GetAll();

        public IEnumerable<TransactionDTO> GetAllWithParams(DateOnly? from, DateOnly? to, string[]? cards, int?[]? bucketIDs);
        public IEnumerable<TransactionDisplayDTO> GetAllDisplayWithParams(DateOnly? from, DateOnly? to, string[]? cards, string?[]? buckets);
        public TransactionDTO GetByID(int id);

        public IEnumerable<TransactionDTO> GetAllAccounted();

        public IEnumerable<TransactionDTO> GetAccountedByDate(DateOnly from, DateOnly to);

        public IEnumerable<TransactionDTO> GetAccountedByCards(string[] cards);

        public IEnumerable<TransactionDTO> GetAccountedByBuckets(int?[] bucketIDs);

        public IEnumerable<TransactionDTO> GetAccountedByDate_Cards(DateOnly from, DateOnly to, string[] cards);

        public IEnumerable<TransactionDTO> GetAccountedByDate_Buckets(DateOnly from, DateOnly to, int?[] bucketIDs);

        public IEnumerable<TransactionDTO> GetAccountedByCards_Buckets(string[] cards, int?[] bucketIDs);

        public IEnumerable<TransactionDTO> GetAccountedByDate_Cards_Buckets(DateOnly from, DateOnly to, string[] cards, int?[] bucketIDs);

        public IEnumerable<TransactionDTO> GetIgnored();

        public Task<ChangeResultPacket<TransactionDTO>> AddTransactions(List<TransactionDTO> items);
    }
}
