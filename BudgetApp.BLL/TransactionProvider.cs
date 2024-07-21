using AutoMapper;
using BudgetApp.Shared;
using BudgetApp.DAL;
using System.Linq.Expressions;
using System.Linq;

namespace BudgetApp.BLL
{
    public class TransactionProvider : ITransactionProvider
    {
        int ignoredBucketID = 25;
        ITransactionRepo _repo;
        ISpendingBucketProvider _bucketProvider;
        private readonly IMapper _mapper;

        public TransactionProvider(ITransactionRepo repo, ISpendingBucketProvider repo2, IMapper mapper)
        {
            _repo = repo;
            _bucketProvider = repo2;
            _mapper = mapper;
        }

        public IEnumerable<TransactionDTO> GetAll()
        {
            var results = _repo.GetAll();
            return results.Select(item => _mapper.Map<TransactionDTO>(item));
        }

        public IEnumerable<TransactionDTO> GetAllWithParams(DateOnly? from, DateOnly? to, string[]? cards, int?[]? bucketIDs)
        {
            if (from == null)
                from = DateOnly.ParseExact("01/01/2000", "MM/dd/yyyy");
            if (to == null)
                to = DateOnly.ParseExact("01/01/2099", "MM/dd/yyyy");

            var results = _repo.GetWhere(t =>(t.TransactionDate >= from && t.TransactionDate <= to));

            if (cards != null)
                results = results.Where(t => cards.Contains(t.Card));
            
            if(bucketIDs != null)
                results = results.Where(t => bucketIDs.Contains(t.BucketId));

            return results.Select(_mapper.Map<TransactionDTO>);
        }

        public IEnumerable<TransactionDisplayDTO> GetAllDisplayWithParams(DateOnly? from, DateOnly? to, string[]? cards, string?[]? buckets)
        {
            IEnumerable<SpendingBucketDTO> SBs = _bucketProvider.GetAll();

            if (from == null)
                from = DateOnly.ParseExact("01/01/2000", "MM/dd/yyyy");
            if (to == null)
                to = DateOnly.ParseExact("01/01/2099", "MM/dd/yyyy");

            var results = _repo.GetWhere(t => (t.TransactionDate >= from && t.TransactionDate <= to));
            if (cards != null)
                results = results.Where(t => cards.Contains(t.Card));
            if (buckets != null)
                results = results.Where(t => buckets.Contains(t.Bucket.BucketLabel));

            //List<TransactionDisplayDTO> ret = new List<TransactionDisplayDTO>();
            //foreach (Transaction t in results)
            //{
            //    TransactionDisplayDTO dis = new TransactionDisplayDTO();
            //    dis.TransactionDate = t.TransactionDate;
            //    dis.Card = t.Card;
            //    dis.BucketLabel = SBs.Where(b => b.BucketId == t.BucketId).Select(b => b.BucketLabel).First();
            //    dis.Amount = t.Amount;
            //    dis.Debit = t.Debit;
            //    dis.Description = t.Description;
            //    dis.Reference = t.Reference;
            //    dis.Priority = t.Priority;
            //    ret.Add(dis);
            //}

            //return ret.OrderBy(td => td.TransactionDate);

            return results.Select(t => _mapper.Map<TransactionDisplayDTO>(t));
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

        public IEnumerable<TransactionDTO> GetAccountedByDate_Buckets(DateOnly from, DateOnly to, int?[] bucketIDs)
        {

            var results = GetAccounted(t => (t.TransactionDate >= from && t.TransactionDate <= to) && bucketIDs.Contains(t.BucketId));
            return results.Select(_mapper.Map<TransactionDTO>);
        }

        public IEnumerable<TransactionDTO> GetAccountedByCards_Buckets(string[] cards, int?[] bucketIDs)
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
