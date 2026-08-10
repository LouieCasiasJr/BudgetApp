using AutoMapper;
using BudgetApp.DAL;
using BudgetApp.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;

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
                from = DateOnly.ParseExact("2000/01/01", "yyyy/MM/dd");
            if (to == null)
                to = DateOnly.ParseExact("2099/01/01", "yyyy/MM/dd");

            var results = _repo.GetWhere(t => (t.TransactionDate >= from && t.TransactionDate <= to));

            if (cards != null)
                results = results.Where(t => cards.Contains(t.Card));

            if (bucketIDs != null)
                results = results.Where(t => bucketIDs.Contains(t.BucketId));

            return results.Select(_mapper.Map<TransactionDTO>);
        }

        public IEnumerable<TransactionDisplayDTO> GetAllDisplayWithParams(DateOnly? from, DateOnly? to, string[]? cards, string?[]? buckets)
        {
            IEnumerable<SpendingBucketDTO> SBs = _bucketProvider.GetAll();
            int?[] IDs = buckets != null ? SBs.Where(b => buckets.Contains(b.BucketLabel)).Select(b => b.BucketId as int?).ToArray() :
                SBs.Select(b => b.BucketId as int?).ToArray();

            if (from == null)
                from = DateOnly.ParseExact("2000/01/01", "yyyy/MM/dd");
            if (to == null)
                to = DateOnly.ParseExact("2099/01/01", "yyyy/MM/dd");

            var results = _repo.GetWhere(t => (t.TransactionDate >= from && t.TransactionDate <= to));
            if (cards != null)
                results = results.Where(t => cards.Contains(t.Card));
            if (buckets != null)
                results = results.Where(t => IDs.Contains(t.BucketId));

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

        private IEnumerable<Transaction> GetAccounted() => GetAccounted(x => true);

        private IEnumerable<Transaction> GetAccounted(Expression<Func<Transaction, bool>> func)
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

        public async Task<ChangeResultPacket<TransactionDTO>> AddTransactions(List<TransactionDTO> items)
        {
            if (items == null || items.Count < 1)
                return null;

            ChangeResultPacket<TransactionDTO> resp = new ChangeResultPacket<TransactionDTO>();
            List<Transaction> trans = new List<Transaction>();

            try
            {
                resp = this.dupeCheck(items);
                if (resp.SucccessSet != null && resp.SucccessSet.Count > 0)
                {
                    trans = resp.SucccessSet.Select(t => _mapper.Map<Transaction>(t)).ToList();
                    _repo.AddRange(trans);
                    await _repo.SaveAsync();
                }

                resp.SuccessCount = resp.SucccessSet?.Count();
                resp.FailureCount = resp.FailureSet?.Count();
            }
            catch (Exception e)
            {
                resp = new ChangeResultPacket<TransactionDTO> { SucccessSet = null, FailureSet = null, SuccessCount = 0, FailureCount = items.Count };
                resp.ErrorMessage = e.InnerException?.ToString() ?? e.Message;
            }

            return resp;
        }


        private ChangeResultPacket<TransactionDTO>  dupeCheck(List<TransactionDTO> transactions)
        {
            var minDate = transactions.Min(t => t.TransactionDate);
            var maxDate = transactions.Max(t => t.TransactionDate);
            var card = transactions[0].Card.Substring(0, 2);
            var existing = _repo.GetWhere(t => t.TransactionDate >= minDate && t.TransactionDate <= maxDate && t.Card.StartsWith(card)).ToList();

            var existingSet = existing
                .Select(t => $"{t.TransactionDate:yyyy-MM-dd}|{t.Card.Trim()}|{t.Description.ToLower()}|{t.Debit}|{t.Amount}|{t.Reference?.ToLower()}")
                .ToHashSet();

            var failure = new List<TransactionDTO>();
            var success = new List<TransactionDTO>();

            foreach (var (tx, index) in transactions.Select((t, i) => (t, i)))
            {
                var fingerprint = $"{tx.TransactionDate:yyyy-MM-dd}|{tx.Card.Trim()}|{tx.Description.ToLower()}|{tx.Debit}|{tx.Amount}|{tx.Reference?.ToLower()}";

                if (existingSet.Contains(fingerprint))
                {
                    tx.Message = "Duplicate in DB";
                    failure.Add(tx);
                    continue;
                }

                success.Add(tx);
            }

            return new ChangeResultPacket<TransactionDTO> { SucccessSet = success, FailureSet = failure };
        }
    }
}
