using LondonStockApi.Models.DTOs;
using LondonStockApi.Services;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LondonStockApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class TradesController : ControllerBase
    {
        private readonly ITradeService _tradeService;
        private readonly ILogger<TradesController> _logger;

        public TradesController(ITradeService tradeService, ILogger<TradesController> logger)
        {
            _tradeService = tradeService;
            _logger = logger;
        }

        /// <summary>
        /// Records a new trade transaction.
        /// Requires JWT Bearer token in Authorization header.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status401Unauthorized)] // Handled by [Authorize]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status403Forbidden)]   // Handled by [Authorize] if policy fails
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RecordTrade([FromBody] TradeInputModel tradeInput)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorViewModel("Invalid trade data provided.", ModelState.ToString()));
            }

            // Get broker ID from the authenticated user's claims
            var authenticatedBrokerId = User.FindFirstValue("broker_id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authenticatedBrokerId))
            {
                _logger.LogWarning("Authenticated user does not have a broker_id or NameIdentifier claim.");
                return Unauthorized(new ErrorViewModel("Unable to identify broker from token."));
            }

            try
            {
                var tradeId = await _tradeService.RecordTradeAsync(tradeInput, authenticatedBrokerId);
                if (tradeId.HasValue)
                {
                    return CreatedAtAction(nameof(RecordTrade), new { id = tradeId.Value }, new { tradeId = tradeId.Value, message = "Trade recorded successfully." });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorViewModel("Failed to record trade."));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording trade for Ticker: {TickerSymbol}", tradeInput.TickerSymbol);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorViewModel("An unexpected error occurred."));
            }
        }
    }
}