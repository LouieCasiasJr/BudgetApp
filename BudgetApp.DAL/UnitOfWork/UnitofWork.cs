using Microsoft.EntityFrameworkCore;

namespace BudgetApp.DAL
{
    public class UnitofWork : BaseUnitOfWork
    {
        public UnitofWork(DbContext context) : base(context) { }
    }
}
