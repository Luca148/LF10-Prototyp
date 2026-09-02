using Microsoft.AspNetCore.Mvc;

namespace LF10.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HarassmentFilterController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] string value)
    {
        if (value.Contains("hate")) value = "love";

        return Ok(value);
    }
}
