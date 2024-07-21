using AutoMapper;
using BudgetApp.DAL;
using BudgetApp.Shared;

namespace BudgetApp.BLL
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            
            
            CreateMap<Transaction, TransactionDTO>().ReverseMap();
            CreateMap<SpendingBucket, SpendingBucketDTO>().ReverseMap();
            CreateMap<MonthlyBudget, MonthlyBudgetDTO>().ReverseMap();

            CreateMap<Transaction, TransactionDisplayDTO>() // uses MapFrom
                .ForMember(td => td.BucketLabel, opt => opt.MapFrom<CustomResolver>());
        }

    }

    public class CustomResolver : IValueResolver<Transaction, TransactionDisplayDTO, string>
    {
        ISpendingBucketProvider _bucketProvider;
        public CustomResolver(ISpendingBucketProvider repo)
        {
            _bucketProvider = repo;
        }
        public string Resolve(Transaction source, TransactionDisplayDTO destination, string member, ResolutionContext context)
        {
            IEnumerable<SpendingBucketDTO> buckets = _bucketProvider.GetAll();
            return buckets.Where(b => b.BucketId == (int)source.BucketId).Select(b => b.BucketLabel).First();
        }
    }
}
