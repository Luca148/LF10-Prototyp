using HarassmentFilter.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Metadata;

namespace LF10.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController(IFilterService filter) : ControllerBase
    {
        private readonly IFilterService _filter = filter;

        [HttpPost("filter")]
        [EndpointSummary("Filtert eine Nachricht auf problematische Inhalte")]
        [EndpointDescription("Nimmt eine Textnachricht entgegen und gibt die bereinigte Version gemäß aktiver Filterregeln zurück.")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Filter(
            [FromBody] string message)
        {
            var result =
                _filter.FilterMessage(message);

            return Ok(result);
        }
    }
}