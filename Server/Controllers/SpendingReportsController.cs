using Microsoft.AspNetCore.Mvc;
using BudgetApp.Shared;
using BudgetApp.DAL;
using BudgetApp.BLL;

namespace BudgetApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SpendingReportController : ControllerBase
{
    ISpendingReportProvider _provider;
    private readonly ILogger<TransactionController> _logger;

    public SpendingReportController(ISpendingReportProvider provider, ILogger<TransactionController> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    [HttpGet("{months}")]
    public ResultPacket<List<SpendingReportDTO>> Get(int months)
    {
        var results = new ResultPacket<List<SpendingReportDTO>>();
        var items = new List<SpendingReportDTO>();

        items.AddRange(_provider.GetReportsToLastMonth(months));
        if(items.Count <= 0)
        {
            results.IsSuccess = false;
            results.Message = $"Failed to return spending reports";
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
