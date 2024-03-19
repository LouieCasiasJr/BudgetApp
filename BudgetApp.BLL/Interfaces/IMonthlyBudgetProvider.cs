using BudgetApp.DAL;
using BudgetApp.Shared;

namespace BudgetApp.BLL
{
    public interface IMonthlyBudgetProvider
    {

        public IEnumerable<MonthlyBudgetDTO> GetAll();

        public IEnumerable<MonthlyBudgetDTO> GetAllByDate(DateOnly date);

        public IEnumerable<MonthlyBudgetDTO> GetAllByDateRange(DateOnly periodStart, DateOnly periodEnd);

        public MonthlyBudgetDTO GetByBucketID_Date(int id, DateOnly date);

        public MonthlyBudgetDTO GetByBucketID(int id);

        public IEnumerable<MonthlyBudgetDTO> GetByBucketIDs(int[] ids);
    }
}
