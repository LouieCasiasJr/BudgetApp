using BudgetApp.BLL;
using BudgetApp.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BudgetApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MonthlyBudgetsController : ControllerBase
    {
        private readonly IMonthlyBudgetProvider _provider;
        private readonly ILogger<MonthlyBudgetsController> _logger;

        public MonthlyBudgetsController(IMonthlyBudgetProvider provider, ILogger<MonthlyBudgetsController> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        [HttpGet]
        public ResultPacket<List<MonthlyBudgetDTO>> GetActive()
        {
            DateOnly td = DateOnly.Parse(DateTime.Today.ToString());
            var results = new ResultPacket<List<MonthlyBudgetDTO>>();
            var items = new List<MonthlyBudgetDTO>();

            items.AddRange(_provider.GetAllByDate(td));
            if (items.Count <= 0)
            {
                results.IsSuccess = false;
                results.Message = $"Failed to return spending buckets";
                _logger.LogWarning("Operation failed: {Reason}", results.Message);
            }
            else
            {
                results.IsSuccess = true;
                results.Data = items;
                results.Message = "";
            }

            return results;
        }
    }
}
