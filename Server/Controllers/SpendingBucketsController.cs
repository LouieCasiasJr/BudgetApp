using Microsoft.AspNetCore.Mvc;
using BudgetApp.Shared;
using BudgetApp.DAL;
using BudgetApp.BLL;

namespace BudgetApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SpendingBucketsController : ControllerBase
{
    private readonly ISpendingBucketProvider _provider;
    private readonly ILogger<SpendingBucketsController> _logger;

    public SpendingBucketsController(ISpendingBucketProvider provider, ILogger<SpendingBucketsController> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    [HttpGet]
    public ResultPacket<List<SpendingBucketDTO>> GetAll()
    {
        var results = new ResultPacket<List<SpendingBucketDTO>>();
        var items = new List<SpendingBucketDTO>();

        items.AddRange(_provider.GetAll());
        if(items.Count <= 0)
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

    [HttpPost("Add")]
    public async Task<IActionResult> Submit(List<SpendingBucketDTO> buckets)
    {
        var failures = new List<SpendingBucketDTO>();
        var validated = new List<SpendingBucketDTO>();

        foreach (SpendingBucketDTO t in buckets)
        {
            try
            {
                validated.Add(t);
            }
            catch (Exception ex)
            {
                //t.Message = ex.Message;
                failures.Add(t);
            }
        }

        return Ok(new ChangeResultPacket<SpendingBucketDTO>
        {
            SuccessCount = validated.Count(),
            FailureCount = failures.Count(),
            FailureSet = failures
        });
    }
}
