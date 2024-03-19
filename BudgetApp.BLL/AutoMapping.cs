using AutoMapper;
using BudgetApp.DAL;
using BudgetApp.Shared;

namespace BudgetApp.BLL
{
    public class AutoMapping: Profile
    {
        public AutoMapping() 
        {
            CreateMap<Transaction, TransactionDTO>().ReverseMap();
            CreateMap<SpendingBucket, SpendingBucketDTO>().ReverseMap();
            CreateMap<MonthlyBudget, MonthlyBudgetDTO>().ReverseMap();
        }

    }
}
