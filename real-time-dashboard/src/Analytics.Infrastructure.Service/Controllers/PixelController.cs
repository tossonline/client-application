using Microsoft.AspNetCore.Mvc;
using Analytics.Application.Models; // Ensure PixelEventModel exists here
using Analytics.Infrastructure.Kafka; // Ensure IKafkaProducer exists here
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Analytics.Infrastructure.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PixelController : ControllerBase
    {
        private readonly IKafkaProducer _kafkaProducer;

        public PixelController(IKafkaProducer kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }

        /// <summary>
        /// Ingests a visit pixel event.
        /// </summary>
        [HttpGet("visit")]
        public async Task<IActionResult> Visit([FromQuery] string playerId, [FromQuery] string bannerTag, [FromQuery] string s, [FromQuery] string b)
        {
            return await HandlePixelEvent("Visit", playerId, bannerTag, s, b);
        }

        /// <summary>
        /// Ingests a registration pixel event.
        /// </summary>
        [HttpGet("registration")]
        public async Task<IActionResult> Registration([FromQuery] string playerId, [FromQuery] string bannerTag, [FromQuery] string s, [FromQuery] string b)
        {
            return await HandlePixelEvent("RegistrationSuccess", playerId, bannerTag, s, b);
        }

        /// <summary>
        /// Ingests a deposit pixel event.
        /// </summary>
        [HttpGet("deposit")]
        public async Task<IActionResult> Deposit([FromQuery] string playerId, [FromQuery] string bannerTag, [FromQuery] string s, [FromQuery] string b)
        {
            return await HandlePixelEvent("DepositSuccess", playerId, bannerTag, s, b);
        }

        /// <summary>
        /// Handles pixel event ingestion, validation, and Kafka publishing.
        /// </summary>
        private async Task<IActionResult> HandlePixelEvent(string eventType, string playerId, string bannerTag, string s, string b)
        {
            if (string.IsNullOrWhiteSpace(playerId) || string.IsNullOrWhiteSpace(bannerTag))
                return BadRequest("Missing required parameters: playerId and bannerTag are required.");

            var metadata = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(s)) metadata["s"] = s;
            if (!string.IsNullOrEmpty(b)) metadata["b"] = b;

            var pixelEvent = new PixelEventModel
            {
                EventType = eventType,
                PlayerId = playerId,
                BannerTag = bannerTag,
                Metadata = metadata,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                await _kafkaProducer.ProduceAsync("pixel-events", pixelEvent);
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
