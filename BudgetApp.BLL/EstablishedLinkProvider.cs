using AutoMapper;
using BudgetApp.DAL;
using BudgetApp.Shared;
using System.Globalization;

namespace BudgetApp.BLL
{
    public class EstablishedLinkProvider : IEstablishedLinkProvider
    {
        IEstablishedLinkRepo _repo;
        private readonly IMapper _mapper;

        public EstablishedLinkProvider(IMapper mapper, IEstablishedLinkRepo repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public IEnumerable<EstablishedLinkDTO> GetAll()
        {
            var results = _repo.GetAll();
            return results.Select(item => _mapper.Map<EstablishedLinkDTO>(item));
        }

        public EstablishedLinkDTO GetByID(int id)
        {
            List<EstablishedLink> EstablishedLinks = new List<EstablishedLink>(_repo.GetWhere(i => i.BucketId == id));
            return EstablishedLinks?.Select(_mapper.Map<EstablishedLinkDTO>).FirstOrDefault();
        }

        public IEnumerable<EstablishedLinkDTO> GetByIDs(int[] ids)
        {
            List<EstablishedLink> EstablishedLinks = new List<EstablishedLink>(_repo.GetWhere(i => ids.Contains(i.BucketId)));
            return EstablishedLinks?.Select(_mapper.Map<EstablishedLinkDTO>);
        }

        public async Task<EstablishedLinkDTO> AddEstablishedLink(EstablishedLinkDTO item)
        {
            if (item == null)
                return null;
            try
            {
                EstablishedLink link = _mapper.Map<EstablishedLink>(item);
                var test = _repo.Add(link);
                await _repo.SaveAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.ToString());
            }

            return item;
        }

        public async Task<List<EstablishedLinkDTO>> AddEstablishedLinks(List<EstablishedLinkDTO> items)
        {
            if (items == null || items.Count > 1)
                return null;
            try
            {
                List<EstablishedLink> links = items.Select(item => _mapper.Map<EstablishedLink>(item)).ToList();
                _repo.AddRange(links);
                await _repo.SaveAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException.ToString());
            }


            return items;
        }
    }
}
