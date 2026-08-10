using BudgetApp.BLL;
using BudgetApp.DAL;
using BudgetApp.Shared;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace BudgetApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class EstablishedLinksController : ControllerBase
{
    private readonly IEstablishedLinkProvider _provider;
    private readonly ILogger<EstablishedLinksController> _logger;

    public EstablishedLinksController(IEstablishedLinkProvider provider, ILogger<EstablishedLinksController> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    [HttpGet]
    public ResultPacket<List<EstablishedLinkDTO>> GetAll()
    {
        var results = new ResultPacket<List<EstablishedLinkDTO>>();
        var items = new List<EstablishedLinkDTO>();

        items.AddRange(_provider.GetAll());
        if(items.Count <= 0)
        {
            results.IsSuccess = false;
            results.Message = $"Failed to return established links";
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
    public async Task<IActionResult> Submit([FromBody] EstablishedLinkDTO link)
    {
        await _provider.AddEstablishedLink(link);

        return Ok(new ChangeResultPacket<EstablishedLinkDTO>
        {
            SuccessCount = 1
        });
    }

    [HttpPost("AddList")]
    public async Task<IActionResult> SubmitList([FromBody] List<EstablishedLinkDTO> links)
    {
        var resp = await _provider.AddEstablishedLinks(links);

        return Ok(resp);
    }
}
