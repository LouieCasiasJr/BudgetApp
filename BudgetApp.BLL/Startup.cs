using BudgetApp.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace BudgetApp.BLL
{
    public static class Startup
    {
        public static IServiceCollection ConfigureBLLServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<BudgetContext>();
            optionsBuilder.UseSqlServer(connectionString);
            services.AddScoped<DbContext, BudgetContext>((sp) => { return new BudgetContext(optionsBuilder.Options); });
            services.AddTransient<IUnitOfWork, UnitofWork>();

            services.AddTransient<IEstablishedLinkRepo, EstablishedLinkRepository>();
            services.AddTransient<IMonthlyBudgetRepo, MonthlyBudgetRepository>();
            services.AddTransient<IMonthlyBudgetProvider, MonthlyBudgetProvider>();
            services.AddTransient<ISpendingBucketRepo, SpendingBucketRepository>();
            services.AddTransient<ISpendingBucketProvider, SpendingBucketProvider>();
            services.AddTransient<ITransactionRepo, TransactionRepository>();
            services.AddTransient<ITransactionProvider, TransactionProvider>();
            services.AddTransient<ISpendingReportProvider, SpendingReportProvider>();

            services.AddAutoMapper(opts => opts.AddProfile<AutoMapping>(), typeof(Startup));

            return services;
        }
    }
}
