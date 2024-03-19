namespace BudgetApp.DAL
{
    public class SpendingBucketRepository : BaseRepository<SpendingBucket>, ISpendingBucketRepo
    {
        public SpendingBucketRepository(IUnitOfWork uow):base(uow) { }
    }
}
