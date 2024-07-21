using Microsoft.AspNetCore.Mvc;
using BudgetApp.Shared;
using BudgetApp.DAL;
using BudgetApp.BLL;
using System.Globalization;

namespace BudgetApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionController : ControllerBase
{
    ITransactionProvider _provider;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(ITransactionProvider provider, ILogger<TransactionController> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    [HttpGet("")]
    public ResultPacket<List<TransactionDTO>> Get(string from = "01/01/2000", string to = "01/01/2099", int? bucket = null)
    {
        DateOnly From = DateOnly.Parse(from);
        DateOnly To = DateOnly.Parse(to);
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
    public ResultPacket<List<TransactionDisplayDTO>> GetAllDisplay(string from = "01/01/2000", string to = "01/01/2099", string? bucket = null)
    {
        DateOnly From = DateOnly.Parse(from);
        DateOnly To = DateOnly.Parse(to);
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
