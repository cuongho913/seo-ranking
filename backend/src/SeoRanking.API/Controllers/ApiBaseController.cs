using Microsoft.AspNetCore.Mvc;

namespace SeoRanking.API.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class ApiBaseController : ControllerBase
{
}