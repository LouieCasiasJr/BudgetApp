using BudgetApp.BLL;
using BudgetApp.DAL;
using BudgetApp.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Globalization;

namespace BudgetApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionProvider _provider;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(ITransactionProvider provider, ILogger<TransactionController> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    [HttpGet("")]
    public ResultPacket<List<TransactionDTO>> Get(string from = "2000/01/01", string to = "2099/01/01", int? bucket = null)
    {
        DateOnly From = DateOnly.ParseExact(from, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        DateOnly To = DateOnly.ParseExact(to, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        int?[] buckets = new int?[1] { bucket };
        var results = new ResultPacket<List<TransactionDTO>>();
        var items = new List<TransactionDTO>();

        if (bucket != null)
            items.AddRange(_provider.GetAccountedByDate_Buckets(From, To, buckets));
        else
            items.AddRange(_provider.GetAccountedByDate(From, To));

        if (items.Count <= 0)
        {
            results.IsSuccess = false;
            results.Message = $"Failed to return transactions {from} to {to}";
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

    [HttpGet("AllDisplay")]
    public ResultPacket<List<TransactionDisplayDTO>> GetAllDisplay(string from = "2000/01/01", string to = "2099/01/01", string? bucket = null)
    {
        DateOnly From = DateOnly.ParseExact(from, "yyyy/MM/dd");
        DateOnly To = DateOnly.ParseExact(to, "yyyy/MM/dd");
        string?[] buckets = new string?[1] { bucket };
        var results = new ResultPacket<List<TransactionDisplayDTO>>();
        var items = new List<TransactionDisplayDTO>();

        if (bucket != null)
            items.AddRange(_provider.GetAllDisplayWithParams(From, To, null, buckets));
        else
            items.AddRange(_provider.GetAllDisplayWithParams(From, To, null, null));

        if (items.Count <= 0)
        {
            results.IsSuccess = false;
            results.Message = $"Failed to return transactions {from} to {to}";
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
    public async Task<IActionResult> Submit([FromBody] List<TransactionDTO> transactions)
    {
        ChangeResultPacket<TransactionDTO> resp = new ChangeResultPacket<TransactionDTO>();

        try
        {
            resp = await _provider.AddTransactions(transactions);
        }
        catch (Exception ex)
        {
            resp.ErrorMessage = ex.InnerException?.ToString() ?? ex.Message;
        }

        return Ok(resp);
    }
}
