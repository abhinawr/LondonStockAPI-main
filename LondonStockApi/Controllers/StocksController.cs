
using LondonStockApi.Models.DTOs;
using LondonStockApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LondonStockApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class StocksController : ControllerBase
    {
        private readonly IStockValuationService _stockValuationService;
        private readonly ILogger<StocksController> _logger;

        public StocksController(IStockValuationService stockValuationService, ILogger<StocksController> logger)
        {
            _stockValuationService = stockValuationService;
            _logger = logger;
        }

        /// <summary>
        /// Get the latest value for a specific stock.
        /// </summary>
        /// <param name="tickerSymbol">The ticker symbol (e.g., "VOD").</param>
        /// <returns>The latest stock value.</returns>
        /// <response code="200">Returns the stock value</response>
        /// <response code="400">If the ticker symbol is empty</response>
        /// <response code="404">If no trades found for the ticker symbol</response>
        [HttpGet("{tickerSymbol}/value")]
        [ProducesResponseType(typeof(StockValueViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStockValue(string tickerSymbol)
        {
            if (string.IsNullOrWhiteSpace(tickerSymbol))
            {
                return BadRequest(new ErrorViewModel("Ticker symbol cannot be empty."));
            }
            var stockValue = await _stockValuationService.GetStockValueAsync(tickerSymbol);
            if (stockValue == null)
            {
                return NotFound(new ErrorViewModel($"No trades found for ticker symbol '{tickerSymbol}'."));
            }
            return Ok(stockValue);
        }

        /// <summary>
        /// Get the latest values for all stocks.
        /// </summary>
        /// <returns>All stock values.</returns>
        /// <response code="200">Returns all stock values</response>
        [HttpGet("values")]
        [ProducesResponseType(typeof(StockValueViewModel[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStockValues()
        {
            var stockValues = await _stockValuationService.GetAllStockValuesAsync();
            return Ok(stockValues);
        }

        /// <summary>
        /// Get the latest values for a range of stocks.
        /// </summary>
        /// <param name="tickers">Comma-separated ticker symbols (e.g., "VOD,BARC").</param>
        /// <returns>Stock values for the specified tickers.</returns>
        /// <response code="200">Returns the stock values for the given tickers</response>
        /// <response code="400">If the tickers parameter is empty or invalid</response>
        [HttpGet("values/range")]
        [ProducesResponseType(typeof(StockValueViewModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStockValuesForRange([FromQuery] string tickers)
        {
            if (string.IsNullOrWhiteSpace(tickers))
            {
                return BadRequest(new ErrorViewModel("Tickers query parameter cannot be empty."));
            }
            var tickerList = tickers.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                    .Select(t => t.ToUpperInvariant()).Distinct().ToList();
            if (!tickerList.Any())
            {
                 return BadRequest(new ErrorViewModel("No valid ticker symbols provided."));
            }
            var stockValues = await _stockValuationService.GetStockValuesForRangeAsync(tickerList);
            return Ok(stockValues);
        }
    }
}