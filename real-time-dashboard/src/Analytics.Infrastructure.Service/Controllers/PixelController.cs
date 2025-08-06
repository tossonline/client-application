using Microsoft.AspNetCore.Mvc;
using MassTransit;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Analytics.Infrastructure.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PixelController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public PixelController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        /// <summary>
        /// Ingests a visit pixel event.
        /// </summary>
        [HttpGet("visit")]
        public async Task<IActionResult> Visit([FromQuery] string playerId, [FromQuery] string bannerTag, [FromQuery] string s, [FromQuery] string b)
        {
            return await HandlePixelEvent("visit", playerId, bannerTag, s, b);
        }

        /// <summary>
        /// Ingests a registration pixel event.
        /// </summary>
        [HttpGet("registration")]
        public async Task<IActionResult> Registration([FromQuery] string playerId, [FromQuery] string bannerTag, [FromQuery] string s, [FromQuery] string b)
        {
            return await HandlePixelEvent("registration", playerId, bannerTag, s, b);
        }

        /// <summary>
        /// Ingests a deposit pixel event.
        /// </summary>
        [HttpGet("deposit")]
        public async Task<IActionResult> Deposit([FromQuery] string playerId, [FromQuery] string bannerTag, [FromQuery] string s, [FromQuery] string b)
        {
            return await HandlePixelEvent("deposit", playerId, bannerTag, s, b);
        }

        /// <summary>
        /// Handles pixel event ingestion, validation, and message publishing.
        /// </summary>
        private async Task<IActionResult> HandlePixelEvent(string eventType, string playerId, string bannerTag, string s, string b)
        {
            if (string.IsNullOrWhiteSpace(playerId) || string.IsNullOrWhiteSpace(bannerTag))
                return BadRequest("Missing required parameters: playerId and bannerTag are required.");

            var metadata = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(s)) metadata["s"] = s;
            if (!string.IsNullOrEmpty(b)) metadata["b"] = b;

            var pixelEvent = new 
            {
                EventType = eventType,
                PlayerId = playerId,
                BannerTag = bannerTag,
                Metadata = metadata,
                Timestamp = DateTime.UtcNow,
                SourceIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()
            };

            try
            {
                await _publishEndpoint.Publish(pixelEvent);
                return Ok(new { status = "Event ingested" });
            }
            catch (Exception ex)
            {
                // Log the error (implement logging as needed)
                return StatusCode(500, new { error = "Failed to ingest event", details = ex.Message });
            }
        }
    }
}
