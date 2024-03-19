using AutoMapper;
using BudgetApp.DAL;
using BudgetApp.Shared;

namespace BudgetApp.BLL
{
    public class SpendingBucketProvider : ISpendingBucketProvider
    {
        ISpendingBucketRepo _repo;
        private readonly IMapper _mapper;

        public SpendingBucketProvider(ISpendingBucketRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public IEnumerable<SpendingBucketDTO> GetAll()
        {
            var results = _repo.GetAll();
            return results.Select(item => _mapper.Map<SpendingBucketDTO>(item));
        }

        public SpendingBucketDTO GetByID(int id) {
            List<SpendingBucket> SpendingBuckets = new List<SpendingBucket>(_repo.GetWhere(i => i.BucketId == id));
            return SpendingBuckets?.Select(_mapper.Map<SpendingBucketDTO>).FirstOrDefault();
        }

        public IEnumerable<SpendingBucketDTO> GetByIDs(int[] ids)
        {
            List<SpendingBucket> SpendingBuckets = new List<SpendingBucket>(_repo.GetWhere(i => ids.Contains(i.BucketId)));
            return SpendingBuckets?.Select(_mapper.Map<SpendingBucketDTO>);
        }
    }
}
