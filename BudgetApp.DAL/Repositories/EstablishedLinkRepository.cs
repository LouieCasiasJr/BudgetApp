namespace BudgetApp.DAL
{
    public class EstablishedLinkRepository : BaseRepository<EstablishedLink>, IEstablishedLinkRepo
    {
        public EstablishedLinkRepository(IUnitOfWork uow):base(uow) { }
    }
}
