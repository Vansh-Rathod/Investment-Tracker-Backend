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
    public class StockRepository : IStockRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public StockRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<List<StockViewModel>>> GetStocks(int stockId = 0, int page = 1, int pageSize = 20, string searchText = "", string sortColumn = "StockName", string sortOrder = "ASC", bool? isEtf = null, int exchangeId = 0)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@StockId", stockId);
                parameters.Add("@Page", page);
                parameters.Add("@PageSize", pageSize);
                parameters.Add("@SearchText", searchText ?? "");
                parameters.Add("@SortColumn", sortColumn ?? "StockName");
                parameters.Add("@SortOrder", sortOrder ?? "ASC");
                parameters.Add("@IsETF", isEtf);
                parameters.Add("@ExchangeId", exchangeId);

                var data = await connection.QueryAsync<StockViewModel>(
                    "GetStocks",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<StockViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Stocks fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Stocks", Core.Enums.Enum.LogLevel.Error, "StockRepository.GetStocks", ex.Message, new Dictionary<string, object>
                {
                    { "StockId", stockId },
                    { "Page", page },
                    { "PageSize", pageSize },
                    { "SearchText", searchText },
                    { "SortColumn", sortColumn },
                    { "SortOrder", sortOrder },
                    { "IsETF", isEtf },
                    { "ExchangeId", exchangeId }
                });
                return DbResponse<List<StockViewModel>>.FailureDbResponse(
                    new List<StockViewModel>(),
                    new List<string> { "Failed to fetch Stocks." },
                    "Exception occurred while fetching Stocks"
                );
            }
        }

        public async Task<DbResponse<int>> InsertUpdateDeleteStock(Stock stock, OperationType operationType)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@OperationType", (int)operationType);
                parameters.Add("@StockId", stock.StockId);
                parameters.Add("@StockName", stock.StockName);
                parameters.Add("@Symbol", stock.Symbol);
                parameters.Add("@ExchangeId", stock.ExchangeId);
                parameters.Add("@ISIN", stock.ISIN);
                parameters.Add("@IsActive", stock.IsActive);
                parameters.Add("@IsETF", stock.IsETF);

                var resultId = await connection.ExecuteScalarAsync<int>(
                    "InsertUpdateDeleteStock",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<int>.SuccessDbResponse(
                    resultId,
                    $"Stock {operationType} operation successful"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to perform Stock operation", Core.Enums.Enum.LogLevel.Error, "StockRepository.InsertUpdateDeleteStock", ex.Message, new Dictionary<string, object>
                {
                    { "OperationType", operationType },
                    { "StockId", stock?.StockId },
                    { "StockName", stock?.StockName }
                });
                return DbResponse<int>.FailureDbResponse(0, new List<string> { "Failed to perform Stock operation." }, "Exception occurred");
            }
        }
        public async Task<DbResponse<List<UserStockHoldingViewModel>>> GetStockHoldings(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);

                var data = await connection.QueryAsync<UserStockHoldingViewModel>(
                    "GetStockHoldings",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<UserStockHoldingViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Stock holdings fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Stock holdings", Core.Enums.Enum.LogLevel.Error, "StockRepository.GetStockHoldings", ex.Message, new Dictionary<string, object>
                {
                    { "UserId", userId }
                });
                return DbResponse<List<UserStockHoldingViewModel>>.FailureDbResponse(
                    new List<UserStockHoldingViewModel>(),
                    new List<string> { "Failed to fetch Stock holdings." },
                    "Exception occurred while fetching holdings"
                );
            }
        }
    }
}
