using LondonStockApi.Models.DTOs;
using System;
using System.Threading.Tasks;

namespace LondonStockApi.Services
{
    public interface ITradeService
    {
        Task<Guid?> RecordTradeAsync(TradeInputModel tradeInput, string authenticatedBrokerId);
    }
}