using LondonStockApi.Data;
using LondonStockApi.Models.DTOs;
using LondonStockApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LondonStockApi.Services
{
    public class StockValuationService : IStockValuationService
    {
        private readonly StockDbContext _context;
        private readonly ILogger<StockValuationService> _logger;

        public StockValuationService(StockDbContext context, ILogger<StockValuationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<StockValueViewModel?> GetStockValueAsync(string tickerSymbol)
        {
            var normalizedTicker = tickerSymbol.ToUpper();
            // Ensure we are querying against the Trade entity
            var stockTrades = _context.Trades.Where(t => t.TickerSymbol == normalizedTicker);

            if (!await stockTrades.AnyAsync())
            {
                _logger.LogInformation("No trades found for ticker: {TickerSymbol}", normalizedTicker);
                return null;
            }
            
            var averagePrice = await stockTrades.AverageAsync(t => t.Price);

            return new StockValueViewModel
            {
                TickerSymbol = normalizedTicker,
                CurrentValue = averagePrice
            };
        }

        public async Task<IEnumerable<StockValueViewModel>> GetAllStockValuesAsync()
        {
            var stockValues = await _context.Trades
                .GroupBy(t => t.TickerSymbol)
                .Select(g => new StockValueViewModel
                {
                    TickerSymbol = g.Key,
                    CurrentValue = g.Average(t => t.Price)
                })
                .OrderBy(s => s.TickerSymbol)
                .ToListAsync();

            return stockValues;
        }

        public async Task<IEnumerable<StockValueViewModel>> GetStockValuesForRangeAsync(IEnumerable<string> tickerSymbols)
        {
            var normalizedTickers = tickerSymbols.Select(t => t.ToUpperInvariant()).Distinct().ToList();

            if (!normalizedTickers.Any())
            {
                return Enumerable.Empty<StockValueViewModel>();
            }

            var stockValues = await _context.Trades
                .Where(t => normalizedTickers.Contains(t.TickerSymbol))
                .GroupBy(t => t.TickerSymbol)
                .Select(g => new StockValueViewModel
                {
                    TickerSymbol = g.Key,
                    CurrentValue = g.Average(t => t.Price)
                })
                .OrderBy(s => s.TickerSymbol)
                .ToListAsync();

            return stockValues;
        }
    }
}