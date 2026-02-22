using Core.CommonModels;
using Core.Entities;
using Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Core.Enums.Enum;

namespace Infrastructure.Interfaces
{
    public interface IMutualFundRepository
    {
        Task<DbResponse<List<MutualFundViewModel>>> GetMutualFunds(int fundId = 0, int amcId = 0, string fundName = null, int categoryId = 0, int categoryType = 0, int page = 1, int pageSize = 20, string searchText = "", string sortColumn = "FundName", string sortOrder = "ASC");
        Task<DbResponse<List<MutualFundHoldingViewModel>>> GetMutualFundHoldings(int userId);
        Task<DbResponse<int>> InsertUpdateDeleteMutualFund(MutualFund mutualFund, OperationType operationType);
    }
}
