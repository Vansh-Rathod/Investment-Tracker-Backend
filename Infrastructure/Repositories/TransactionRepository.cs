using Core.DTOs;
using Dapper;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TransactionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<TransactionDTO>> GetUserTransactionsAsync(
            int userId,
            int portfolioId = 0,
            int assetId = 0,
            int assetTypeId = 0,
            int transactionType = 0,
            DateTime? fromDate = null,
            DateTime? toDate = null)
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

            var result = await connection.QueryAsync<TransactionDTO>(
                "GetUserTransactions",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<IEnumerable<TransactionDTO>> GetStockTransactionsAsync(int userId, int portfolioId = 0)
        {
            return await GetUserTransactionsAsync(userId, portfolioId, assetTypeId: 1);
        }

        public async Task<IEnumerable<TransactionDTO>> GetMutualFundTransactionsAsync(int userId, int portfolioId = 0)
        {
            return await GetUserTransactionsAsync(userId, portfolioId, assetTypeId: 2);
        }

        public async Task<IEnumerable<TransactionDTO>> GetSIPTransactionsAsync(int userId, int portfolioId = 0)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PortfolioId", portfolioId);
            parameters.Add("@AssetId", 0);
            parameters.Add("@AssetTypeId", 0);
            parameters.Add("@TransactionType", 0);
            parameters.Add("@FromDate", null);
            parameters.Add("@ToDate", null);

            var result = await connection.QueryAsync<TransactionDTO>(
                "GetUserTransactions",
                parameters,
                commandType: CommandType.StoredProcedure);

            // Filter for SIP transactions only
            return result.Where(t => t.SourceType == "SIP");
        }
    }
}
