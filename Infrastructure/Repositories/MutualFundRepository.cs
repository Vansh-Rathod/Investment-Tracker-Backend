using Core.CommonModels;
using Core.ViewModels;
using Dapper;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Core.Entities;
using static Core.Enums.Enum;

namespace Infrastructure.Repositories
{
    public class MutualFundRepository : IMutualFundRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public MutualFundRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<List<MutualFundViewModel>>> GetMutualFunds(int fundId = 0, int amcId = 0, string fundName = null, int categoryId = 0, int categoryType = 0, int page = 1, int pageSize = 20, string searchText = "", string sortColumn = "FundName", string sortOrder = "ASC")
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@FundId", fundId);
                parameters.Add("@AssetManagementCompanyId", amcId);
                parameters.Add("@FundName", fundName);
                parameters.Add("@CategoryId", categoryId);
                parameters.Add("@CategoryType", categoryType);
                parameters.Add("@Page", page);
                parameters.Add("@PageSize", pageSize);
                parameters.Add("@SearchText", searchText ?? "");
                parameters.Add("@SortColumn", sortColumn ?? "FundName");
                parameters.Add("@SortOrder", sortOrder ?? "ASC");

                var data = await connection.QueryAsync<MutualFundViewModel>(
                    "GetMutualFunds",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<MutualFundViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Mutual Funds fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Mutual Funds", Core.Enums.Enum.LogLevel.Error, "MutualFundRepository.GetMutualFunds", ex, new Dictionary<string, object>
                {
                    { "FundId", fundId },
                    { "AMCId", amcId },
                    { "FundName", fundName },
                    { "CategoryId", categoryId },
                    { "CategoryType", categoryType },
                    { "Page", page },
                    { "PageSize", pageSize },
                    { "SearchText", searchText },
                    { "SortColumn", sortColumn },
                    { "SortOrder", sortOrder }
                });
                return DbResponse<List<MutualFundViewModel>>.FailureDbResponse(
                    new List<MutualFundViewModel>(),
                    new List<string> { "Failed to fetch Mutual Funds." },
                    "Exception occurred while fetching Mutual Funds"
                );
            }
        }

        public async Task<DbResponse<int>> InsertUpdateDeleteMutualFund(MutualFund mutualFund, OperationType operationType)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@OperationType", (int)operationType);
                parameters.Add("@FundId", mutualFund.FundId);
                parameters.Add("@AmcId", mutualFund.AmcId);
                parameters.Add("@FundName", mutualFund.FundName);
                parameters.Add("@Category", mutualFund.Category);
                parameters.Add("@ISIN", mutualFund.ISIN);
                parameters.Add("@IsActive", mutualFund.IsActive);

                var resultId = await connection.ExecuteScalarAsync<int>(
                    "InsertUpdateDeleteMutualFund",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<int>.SuccessDbResponse(
                    resultId,
                    $"Mutual Fund {operationType} operation successful"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to perform Mutual Fund operation", Core.Enums.Enum.LogLevel.Error, "MutualFundRepository.InsertUpdateDeleteMutualFund", ex, new Dictionary<string, object>
                {
                    { "OperationType", operationType },
                    { "FundId", mutualFund?.FundId },
                    { "FundName", mutualFund?.FundName }
                });
                return DbResponse<int>.FailureDbResponse(0, new List<string> { "Failed to perform Mutual Fund operation." }, "Exception occurred");
            }
        }
    }
}
