using Core.CommonModels;
using Core.DTOs;
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
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public TransactionRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<List<TransactionViewModel>>> GetUserTransactionsAsync(
            int userId,
            int portfolioId = 0,
            int assetId = 0,
            int assetTypeId = 0,
            int transactionType = 0,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@PortfolioId", portfolioId);
                parameters.Add("@AssetId", assetId);
                parameters.Add("@AssetTypeId", assetTypeId);
                parameters.Add("@TransactionType", transactionType);
                parameters.Add("@FromDate", fromDate);
                parameters.Add("@ToDate", toDate);

                var data = await connection.QueryAsync<TransactionViewModel>(
                    "GetUserTransactions",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<TransactionViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Transactions fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Transactions", Core.Enums.Enum.LogLevel.Error, "TransactionRepository.GetUserTransactionsAsync", ex, new Dictionary<string, object>
                {
                    { "UserId", userId },
                    { "PortfolioId", portfolioId },
                    { "AssetId", assetId },
                    { "AssetTypeId", assetTypeId },
                    { "TransactionType", transactionType },
                    { "FromDate", fromDate },
                    { "ToDate", toDate }
                });
                return DbResponse<List<TransactionViewModel>>.FailureDbResponse(
                    new List<TransactionViewModel>(),
                    new List<string> { "Failed to fetch Transactions." },
                    "Exception occurred while fetching Transactions"
                );
            }
        }

        public async Task<DbResponse<int>> InsertTransaction(Transaction transaction)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@PortfolioId", transaction.PortfolioId);
                parameters.Add("@AssetTypeId", transaction.AssetTypeId);
                parameters.Add("@AssetId", transaction.AssetId);
                parameters.Add("@TransactionType", transaction.TransactionType);
                parameters.Add("@Units", transaction.Units);
                parameters.Add("@Price", transaction.Price);
                parameters.Add("@Amount", transaction.Amount);
                parameters.Add("@TransactionDate", transaction.TransactionDate);
                parameters.Add("@SourceType", transaction.SourceType);
                parameters.Add("@SourceId", transaction.SourceId);

                var resultId = await connection.ExecuteScalarAsync<int>(
                    "InsertTransaction",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<int>.SuccessDbResponse(
                    resultId,
                    "Transaction inserted successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to insert Transaction", Core.Enums.Enum.LogLevel.Error, "TransactionRepository.InsertTransaction", ex, new Dictionary<string, object>
                {
                    { "PortfolioId", transaction?.PortfolioId },
                    { "AssetId", transaction?.AssetId },
                    { "TransactionType", transaction?.TransactionType }
                });
                return DbResponse<int>.FailureDbResponse(0, new List<string> { "Failed to insert Transaction." }, "Exception occurred");
            }
        }
    }
}
