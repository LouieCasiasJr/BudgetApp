using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BudgetApp.DAL
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepo
    {
        public TransactionRepository(IUnitOfWork uow):base(uow) { }

    }
}
