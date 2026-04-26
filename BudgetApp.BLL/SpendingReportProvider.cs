using AutoMapper;
using BudgetApp.DAL;
using BudgetApp.Shared;
using System.Globalization;

namespace BudgetApp.BLL
{
    public class SpendingReportProvider : ISpendingReportProvider
    {
        int incomeBucketID = 1;
        int ignoreBucketID = 25;
        //int savingID = 23;
        ITransactionProvider _transactionProvider;
        ISpendingBucketProvider _bucketProvider;
        IMonthlyBudgetProvider _monthlyBudgetProvider;

        private readonly IMapper _mapper;

        public SpendingReportProvider(ITransactionProvider repo, ISpendingBucketProvider repo2, IMonthlyBudgetProvider repo3, 
            IMapper mapper)
        {
            _transactionProvider = repo;
            _bucketProvider = repo2;
            _monthlyBudgetProvider = repo3;
            _mapper = mapper;
        }

        public List<SpendingReportDTO> GetReportsToLastMonth(int numMonths = 12)
        {
            DateTime s = DateTime.Now.AddMonths(-(numMonths));
            DateOnly start = DateOnly.FromDateTime(new DateTime(s.Year, s.Month, 1));

            IEnumerable<TransactionDTO> transactions = _transactionProvider.GetAccountedByDate(start, start.AddMonths(numMonths).AddDays(-1));
            IEnumerable<MonthlyBudgetDTO> budgets = _monthlyBudgetProvider.GetAllByDateRange(start, start.AddMonths(numMonths).AddDays(-1));

            List<SpendingReportDTO> reports = BuildByMonth(start, transactions, budgets, numMonths);

            if (numMonths >= 6)
                reports.Add(BuildAggregateReport(reports, 6));
            if (numMonths >= 12)
                reports.Add(BuildAggregateReport(reports, 12));
            if (numMonths > 18)
                reports.Add(BuildAggregateReport(reports, 18));

            reports.Reverse();
            return reports;
        }

        private SpendingReportDTO BuildAggregateReport(List<SpendingReportDTO> reports, int numMonths)
        {
            string[] periods = new string[numMonths];

            // Make a list of months to include in the aggregate report
            for (int i = 0; i < numMonths; i++)
            {
                DateTime prevMonth = DateTime.Now.AddMonths(-(i + 1));
                periods[i] = prevMonth.ToString("MMMM yy", CultureInfo.InvariantCulture);
            }
            IEnumerable<SpendingReportDTO> reps = reports.Where(r => periods.Contains(r.Period));

            SpendingReportDTO agg = new SpendingReportDTO();
            agg.Period = $"Average {numMonths}";
            agg.SummaryDisplay = null;
            agg.Income = decimal.Round((reps.Sum(r => r.Income) / numMonths), 2, MidpointRounding.AwayFromZero);
            agg.Budgeted = decimal.Round((reps.Sum(r => r.Budgeted) / numMonths), 2, MidpointRounding.AwayFromZero);
            agg.CardTotals = new Dictionary<string, decimal>();
            agg.Deltas = new List<SpendingBucketResultDTO>();

            string latest = periods[0];
            foreach (SpendingReportDTO rep in reps)
            {
                foreach (KeyValuePair<string, decimal> ct in rep.CardTotals)
                {
                    if (agg.CardTotals.TryGetValue(ct.Key, out decimal z))
                        agg.CardTotals[ct.Key] += ct.Value;
                    else
                        agg.CardTotals.Add(ct.Key, ct.Value);
                }

                bool prev = rep.Period == latest;
                foreach (SpendingBucketResultDTO dt in rep.Deltas)
                {
                    if (agg.Deltas.Any(d => d.BucketLabel == dt.BucketLabel))
                    {
                        SpendingBucketResultDTO sbr = agg.Deltas.Where(d => d.BucketLabel == dt.BucketLabel).First();
                        if (prev)
                            sbr.Budget = dt.Budget;
                        sbr.Delta += dt.Delta;
                    }
                    else
                        agg.Deltas.Add(new SpendingBucketResultDTO { BucketLabel = dt.BucketLabel, DefaultPriority = dt.DefaultPriority, 
                            DisplayOrder = dt.DisplayOrder, Delta = dt.Delta, Budget = dt.Budget });  
                }
            }

            foreach (KeyValuePair<string, decimal> ct in agg.CardTotals)
            {
                agg.CardTotals[ct.Key] = decimal.Round((ct.Value / numMonths), 2, MidpointRounding.AwayFromZero);
            }

            foreach (SpendingBucketResultDTO dt in agg.Deltas)
            {
                dt.Delta = decimal.Round((dt.Delta / numMonths), 2, MidpointRounding.AwayFromZero);
            }

            agg.Deltas = agg.Deltas.OrderBy(x => x.DisplayOrder).ToList();

            return agg;
        }

        private List<SpendingReportDTO> BuildByMonth(DateOnly start, IEnumerable<TransactionDTO> transactions,
            IEnumerable<MonthlyBudgetDTO> budgets, int months)
        {
            List<SpendingReportDTO> monthlyReports = new List<SpendingReportDTO>();
            IEnumerable<SpendingBucketDTO> buckets = _bucketProvider.GetAll();
            buckets = buckets.Where(b => b.BucketId != incomeBucketID && b.BucketId != ignoreBucketID);

            for (int i = 0; i < months; i++)
            {
                DateOnly d = start.AddMonths(i);
                IEnumerable<TransactionDTO> tranSet = transactions
                    .Where(x => x.TransactionDate.Year == d.Year && x.TransactionDate.Month == d.Month);

                if (tranSet != null && tranSet.Count() > 0)
                {
                    Dictionary<string, decimal> bucketAmounts = new Dictionary<string, decimal>();
                    Dictionary<string, decimal> cardAmounts = new Dictionary<string, decimal>();
                    List<TransactionDisplayDTO>? SummaryDisplay = new List<TransactionDisplayDTO>();

                    SpendingReportDTO report = new SpendingReportDTO();
                    report.Period = d.ToString("MMMM yy", CultureInfo.InvariantCulture);
                    report.Income = tranSet.Where(t => t.BucketId == incomeBucketID).Sum(a => a.Amount);
                    tranSet = tranSet.Where(t => t.BucketId != incomeBucketID);


                    foreach (TransactionDTO t in tranSet)
                    {
                        TransactionDisplayDTO display = BuildTransactionDisplay(buckets, t);
                        SummaryDisplay.Add(display);

                        string label = display.BucketLabel;
                        if (bucketAmounts.TryGetValue(label, out decimal b))
                            bucketAmounts[label] += t.Amount;
                        else
                            bucketAmounts.Add(label, t.Amount);

                        if (cardAmounts.TryGetValue(t.Card, out decimal c))
                            cardAmounts[t.Card] += t.Amount;
                        else
                            cardAmounts.Add(t.Card, t.Amount);
                    }

                    report.SummaryDisplay = SummaryDisplay;
                    report.Deltas = BuildDeltas(budgets, buckets, bucketAmounts, d);
                    report.Budgeted = report.Deltas.Last().Budget;
                    report.CardTotals = cardAmounts;
                    monthlyReports.Add(report);
                }
            }

            return monthlyReports;
        }

        private List<SpendingBucketResultDTO> BuildDeltas(IEnumerable<MonthlyBudgetDTO> budgets, IEnumerable<SpendingBucketDTO> buckets,
            Dictionary<string, decimal> bucketAmounts, DateOnly d)
        {
            decimal budgeted = 0;
            List<SpendingBucketResultDTO> sbrs = new List<SpendingBucketResultDTO>();

            foreach (SpendingBucketDTO b in buckets)
            {
                MonthlyBudgetDTO budget = budgets
                    .Where(bg => bg.BucketId == b.BucketId && (bg.StartDate <= d && (bg.EndDate == null || bg.EndDate >= d))).First();

                // Previous calc excluded savings amounts from the sum of spending, this imbalanced the reports -
                // specific transactions can be ignored if desired
                // if (budget.BucketId != savingID)
                budgeted += budget.Amount;

                SpendingBucketResultDTO sbr = new SpendingBucketResultDTO();
                sbr.BucketLabel = b.BucketLabel;
                sbr.DefaultPriority = b.DefaultPriority;
                sbr.DisplayOrder = b.DisplayOrder;
                sbr.Budget = budget.Amount;

                if (bucketAmounts.TryGetValue(b.BucketLabel, out decimal amount))
                    sbr.Delta = amount - budget.Amount;
                else
                    sbr.Delta = 0 - budget.Amount;

                sbrs.Add(sbr);
            }

            SpendingBucketResultDTO total = new SpendingBucketResultDTO();
            total.BucketLabel = "TOTAL";
            total.DefaultPriority = 0;
            total.DisplayOrder = 500;
            total.Delta = sbrs.Select(x => x.Delta).Sum();

            // Previous calc excluded savings amounts from the sum of spending, this imbalanced the reports -
            // specific transactions can be ignored if desired
            // total.Delta = sbrs.Where(x => x.BucketLabel != "Savings").Select(x => x.Delta).Sum();
            total.Budget = budgeted;
            sbrs.Add(total);

            sbrs = sbrs.OrderBy(x => x.DisplayOrder).ToList();

            return sbrs;
        }

        private TransactionDisplayDTO BuildTransactionDisplay(IEnumerable<SpendingBucketDTO> buckets, TransactionDTO t)
        {
            if (!t.Debit)
                t.Amount = t.Amount * -1;

            TransactionDisplayDTO display = new TransactionDisplayDTO();
            display.Amount = t.Amount;
            display.Debit = t.Debit;
            display.TransactionDate = t.TransactionDate;
            display.Card = t.Card;
            display.Description = t.Description;
            display.Reference = t.Reference;
            display.BucketLabel = buckets.Where(b => b.BucketId == (int)t.BucketId).Select(b => b.BucketLabel).First();
            display.Priority = t.Priority;
            return display;
        }

    }
}
