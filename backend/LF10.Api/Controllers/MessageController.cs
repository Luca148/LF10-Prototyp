using HarassmentFilter.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace LF10.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController(IHarassmentFilterService filter) : ControllerBase
    {
        private readonly IHarassmentFilterService _filter = filter;

        [HttpPost("filter")]
        public IActionResult Filter(
            [FromBody] string message)
        {
            var result =
                _filter.FilterMessage(message);

            return Ok(result);
        }
    }
}