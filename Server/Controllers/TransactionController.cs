using Microsoft.AspNetCore.Mvc;
using BudgetApp.Shared;
using BudgetApp.DAL;
using BudgetApp.BLL;

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

    [HttpGet("dates")]
    public ResultPacket<List<TransactionDTO>> Get(string from, string to)
    {
        DateOnly From = DateOnly.Parse(from);
        DateOnly To = DateOnly.Parse(to);
        var results = new ResultPacket<List<TransactionDTO>>();
        var items = new List<TransactionDTO>();

        items.AddRange(_provider.GetAccountedByDate(From, To));
        if(items.Count <= 0)
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
