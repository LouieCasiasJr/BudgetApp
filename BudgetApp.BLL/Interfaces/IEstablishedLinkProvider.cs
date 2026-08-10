using BudgetApp.Shared;

namespace BudgetApp.BLL
{
    public interface IEstablishedLinkProvider
    {
        public IEnumerable<EstablishedLinkDTO> GetAll();

        public EstablishedLinkDTO GetByID(int id);

        public IEnumerable<EstablishedLinkDTO> GetByIDs(int[] ids);

        public Task<EstablishedLinkDTO> AddEstablishedLink(EstablishedLinkDTO item);

        public Task<List<EstablishedLinkDTO>> AddEstablishedLinks(List<EstablishedLinkDTO> items);
    }
}
