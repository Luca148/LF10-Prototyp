using Microsoft.AspNetCore.Mvc;

namespace LF10.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HarassmentFilterController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] string value)
    {
        string input = value;
        string output = "";

        if (input.Contains("hate"))
        {
            output = input.Replace("hate", "love");
        }

        return Ok(output);
    }
}
