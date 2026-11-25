using HarassmentFilter.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Metadata;

namespace LF10.Api.Controllers
{
    /// <summary>
    /// Endpunkte zum Prüfen und Filtern von Chat-Nachrichten.
    /// </summary>
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
        /// <summary>
        /// Prüft den Nachrichtentext mit dem Harassment-Filter.
        /// </summary>
        /// <param name="message">Der zu prüfende Nachrichtentext.</param>
        /// <returns>Die gefilterte Nachricht.</returns>
        public IActionResult Filter(
            [FromBody] string message)
        {
            var result =
                _filter.FilterMessage(message);

            return Ok(result);
        }
    }
}