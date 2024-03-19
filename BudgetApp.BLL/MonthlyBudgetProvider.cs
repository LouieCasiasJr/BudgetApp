using AutoMapper;
using BudgetApp.DAL;
using BudgetApp.Shared;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BudgetApp.BLL
{
    public class MonthlyBudgetProvider : IMonthlyBudgetProvider
    {
        IMonthlyBudgetRepo _repo;
        private readonly IMapper _mapper;

        public MonthlyBudgetProvider(IMonthlyBudgetRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public IEnumerable<MonthlyBudgetDTO> GetAll()
        {
            var results = _repo.GetAll();
            return results.Select(item => _mapper.Map<MonthlyBudgetDTO>(item));
        }
    
        public IEnumerable<MonthlyBudgetDTO> GetAllByDate(DateOnly date)
        {
            var results = _repo.GetWhere(i => i.StartDate <= date && (i.EndDate == null || i.EndDate >= date));
            return results.Select(item => _mapper.Map<MonthlyBudgetDTO>(item));
        }

        public IEnumerable<MonthlyBudgetDTO> GetAllByDateRange(DateOnly periodStart, DateOnly periodEnd)
        {
            var results = _repo.GetWhere(i => periodEnd >= i.StartDate && (periodStart <= i.EndDate || i.EndDate == null));
            return results.Select(item => _mapper.Map<MonthlyBudgetDTO>(item));
        }

        public MonthlyBudgetDTO GetByBucketID_Date(int id, DateOnly date)
        {
            List<MonthlyBudget> MonthlyBudgets = new List<MonthlyBudget>(_repo.
                GetWhere(i => (i.StartDate <= date && (i.EndDate == null || i.EndDate >= date)) && i.BucketId == id));
            return MonthlyBudgets?.Select(_mapper.Map<MonthlyBudgetDTO>).FirstOrDefault();
        }

        public MonthlyBudgetDTO GetByBucketID(int id)
        {
            List<MonthlyBudget> MonthlyBudgets = new List<MonthlyBudget>(_repo.GetWhere(i => i.BucketId == id));
            return MonthlyBudgets?.Select(_mapper.Map<MonthlyBudgetDTO>).FirstOrDefault();
        }

        public IEnumerable<MonthlyBudgetDTO> GetByBucketIDs(int[] ids)
        {
            List<MonthlyBudget> MonthlyBudgets = new List<MonthlyBudget>(_repo.GetWhere(i => ids.Contains(i.BucketId)));
            return MonthlyBudgets.Select(_mapper.Map<MonthlyBudgetDTO>);
        }
    }
}
