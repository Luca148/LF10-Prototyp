using LF10.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LF10.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HarassmentFilterController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] string value)
    {
        return Ok(HarassmentFilterService.FilterMessage(value));
    }
}
