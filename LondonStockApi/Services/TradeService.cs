using LondonStockApi.Data;
using LondonStockApi.Models.DTOs;
using LondonStockApi.Models.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LondonStockApi.Services
{
    public class TradeService : ITradeService
    {
        private readonly StockDbContext _context;
        private readonly ILogger<TradeService> _logger;

        public TradeService(StockDbContext context, ILogger<TradeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid?> RecordTradeAsync(TradeInputModel tradeInput, string authenticatedBrokerId)
        {
            // Optionally, you can validate if tradeInput.BrokerId matches authenticatedBrokerId
            // if (tradeInput.BrokerId != authenticatedBrokerId)
            // {
            //     _logger.LogWarning("Mismatch between submitted BrokerId ({SubmittedBrokerId}) and authenticated user ({AuthenticatedBrokerId}).",
            //         tradeInput.BrokerId, authenticatedBrokerId);
            //     // Decide how to handle: reject, or use authenticatedBrokerId
            // }

            var trade = new Trade
            {
                TradeId = Guid.NewGuid(),
                TickerSymbol = tradeInput.TickerSymbol.ToUpper(),
                Price = tradeInput.Price,
                Shares = tradeInput.Shares,
                BrokerId = authenticatedBrokerId, // Use the ID from the authenticated token
                Timestamp = DateTime.UtcNow
            };

            _context.Trades.Add(trade);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Trade recorded: {TradeId} for Ticker: {TickerSymbol} by Broker: {BrokerId}",
                    trade.TradeId, trade.TickerSymbol, trade.BrokerId);
                return trade.TradeId;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error saving trade to database for Ticker: {TickerSymbol}", tradeInput.TickerSymbol);
                return null;
            }
        }
    }
}