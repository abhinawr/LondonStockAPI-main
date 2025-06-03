using LondonStockApi.Models.DTOs; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LondonStockApi.Services
{
    public interface IStockValuationService
    {
        Task<StockValueViewModel?> GetStockValueAsync(string tickerSymbol);
        Task<IEnumerable<StockValueViewModel>> GetAllStockValuesAsync();
        Task<IEnumerable<StockValueViewModel>> GetStockValuesForRangeAsync(IEnumerable<string> tickerSymbols);
    }
}