using Microsoft.AspNetCore.Mvc;
using BudgetApp.Shared;
using BudgetApp.DAL;
using BudgetApp.BLL;

namespace BudgetApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SpendingReportController : ControllerBase
{
    private readonly ISpendingReportProvider _provider;
    private readonly ILogger<SpendingReportController> _logger;

    public SpendingReportController(ISpendingReportProvider provider, ILogger<SpendingReportController> logger)
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
