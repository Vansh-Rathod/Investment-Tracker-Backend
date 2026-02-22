using Core.CommonModels;
using Core.ViewModels;
using Core.Entities;
using static Core.Enums.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IStockRepository
    {
        Task<DbResponse<List<StockViewModel>>> GetStocks(int stockId = 0, int page = 1, int pageSize = 20, string searchText = "", string sortColumn = "StockName", string sortOrder = "ASC", bool? isEtf = null, int exchangeId = 0);
        Task<DbResponse<List<UserStockHoldingViewModel>>> GetStockHoldings(int userId);
        Task<DbResponse<int>> InsertUpdateDeleteStock(Stock stock, OperationType operationType);
    }
}
