using HarassmentFilter.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LF10.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController(IFilterService filter) : ControllerBase
    {
        private readonly IFilterService _filter = filter;

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