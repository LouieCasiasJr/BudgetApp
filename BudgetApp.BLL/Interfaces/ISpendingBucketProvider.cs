using BudgetApp.Shared;

namespace BudgetApp.BLL
{
    public interface ISpendingBucketProvider
    {
        public IEnumerable<SpendingBucketDTO> GetAll();

        public SpendingBucketDTO GetByID(int id);

        public IEnumerable<SpendingBucketDTO> GetByIDs(int[] ids);
    }
}
