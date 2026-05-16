using BudgetApp.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class BudgetContextFactory : IDesignTimeDbContextFactory<BudgetContext>
{
	public BudgetContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<BudgetContext>();
		optionsBuilder.UseSqlServer("Server=localhost;Database=budgetapp;Trusted_Connection=True;");

		return new BudgetContext(optionsBuilder.Options);
	}
}