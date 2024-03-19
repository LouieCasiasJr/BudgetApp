using AutoMapper;
using BudgetApp.Shared;
using BudgetApp.DAL;
using System.Linq.Expressions;

namespace BudgetApp.BLL
{
    public class TransactionProvider : ITransactionProvider
    {
        int ignoredBucketID = 25;
        ITransactionRepo _repo;
        private readonly IMapper _mapper;

        public TransactionProvider(ITransactionRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public IEnumerable<TransactionDTO> GetAll()
        {
            var results = _repo.GetAll();
            return results.Select(item => _mapper.Map<TransactionDTO>(item));
        }

        public TransactionDTO GetByID(int id)
        {
            List<Transaction> transactions = new List<Transaction>(_repo.GetWhere(i => i.TransactionId == id));
            return transactions.Select(_mapper.Map<TransactionDTO>).First();
        }

        public IEnumerable<TransactionDTO> GetAllAccounted()
        {
            var results = GetAccounted();
            return results.Select(item => _mapper.Map<TransactionDTO>(item));
        }

        public IEnumerable<TransactionDTO> GetAccountedByDate(DateOnly from, DateOnly to)
        {
            var results = GetAccounted(t => t.TransactionDate >= from && t.TransactionDate <= to);
            return results.Select(_mapper.Map<TransactionDTO>);
        }

        public IEnumerable<TransactionDTO> GetAccountedByCards(string[] cards)
        {
            var results = GetAccounted(t => cards.Contains(t.Card));
            return results.Select(_mapper.Map<TransactionDTO>);
        }

        public IEnumerable<TransactionDTO> GetAccountedByDate_Cards(DateOnly from, DateOnly to, string[] cards)
        {
            var results = GetAccounted(t => (t.TransactionDate >= from && t.TransactionDate <= to) && cards.Contains(t.Card));
            return results.Select(_mapper.Map<TransactionDTO>);
        }

        public IEnumerable<TransactionDTO> GetAccountedByBuckets(int?[] bucketIDs)
        {
            var results = GetAccounted(t => bucketIDs.Contains(t.BucketId));
            return results.Select(_mapper.Map<TransactionDTO>);
        }

        public IEnumerable<TransactionDTO> GetAccountedByDate_Buckets(DateOnly from, DateOnly to, string[] cards, int?[] bucketIDs)
        {
            var results = GetAccounted(t => (t.TransactionDate >= from && t.TransactionDate <= to) && bucketIDs.Contains(t.BucketId));
            return results.Select(_mapper.Map<TransactionDTO>);
        }

        public IEnumerable<TransactionDTO> GetAccountedByCards_Buckets(DateOnly from, DateOnly to, string[] cards, int?[] bucketIDs)
        {
            var results = GetAccounted(t => cards.Contains(t.Card) && bucketIDs.Contains(t.BucketId));
            return results.Select(_mapper.Map<TransactionDTO>);
        }

        public IEnumerable<TransactionDTO> GetAccountedByDate_Cards_Buckets(DateOnly from, DateOnly to, string[] cards, int?[] bucketIDs)
        {
            var results = GetAccounted(t =>
                (t.TransactionDate >= from && t.TransactionDate <= to) && cards.Contains(t.Card) && bucketIDs.Contains(t.BucketId));
            return results.Select(_mapper.Map<TransactionDTO>);
        }

        private IEnumerable<Transaction> GetAccounted(Expression<Func<Transaction, bool>> func = null)
        {
            IEnumerable<Transaction> acct;
            if (func != null)
                acct = _repo.GetWhere(func).Where(t => t.BucketId != ignoredBucketID);
            else
                acct = _repo.GetWhere(t => t.BucketId != ignoredBucketID);

            return acct;
        }

        public IEnumerable<TransactionDTO> GetIgnored()
        {
            var results = _repo.GetWhere(t => t.BucketId == ignoredBucketID);
            return results.Select(_mapper.Map<TransactionDTO>);
        }
    }
}
