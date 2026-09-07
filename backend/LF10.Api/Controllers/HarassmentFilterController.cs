using LF10.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LF10.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HarassmentFilterController(HarassmentFilterService filterService) : ControllerBase
{
    private readonly HarassmentFilterService _filterService = filterService;

    [HttpPost]
    public IActionResult Post([FromBody] string value)
    {
        return Ok(_filterService.FilterMessage(value));
    }
}
