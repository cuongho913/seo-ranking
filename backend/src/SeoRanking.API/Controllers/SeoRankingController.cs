using MediatR;
using Microsoft.AspNetCore.Mvc;
using SeoRanking.Application.Queries;
using SeoRanking.Domain.Enums;

namespace SeoRanking.API.Controllers;

public class SeoRankingController(IMediator mediator) : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetResult([FromQuery] string keyword, [FromQuery] string targetUrl,
        [FromQuery] IEnumerable<SearchEngine> searchEngines)
    {
        var result = await mediator.Send(new SeoRankingQuery.SeoRankingRequest
        {
            SearchEngines = searchEngines,
            TargetUrl = targetUrl,
            Keyword = keyword
        });
        return Ok(result);
    }
}