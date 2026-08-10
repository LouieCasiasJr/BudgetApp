using BudgetApp.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class BudgetContextFactory : IDesignTimeDbContextFactory<BudgetContext>
{
	public BudgetContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<BudgetContext>();
		optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=BudgetApp;integrated security=True;TrustServerCertificate=True;");

		return new BudgetContext(optionsBuilder.Options);
	}
}