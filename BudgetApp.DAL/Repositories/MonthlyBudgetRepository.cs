namespace BudgetApp.DAL
{
    public class MonthlyBudgetRepository : BaseRepository<MonthlyBudget>, IMonthlyBudgetRepo
    {
        public MonthlyBudgetRepository(IUnitOfWork uow):base(uow) { }
    }
}
