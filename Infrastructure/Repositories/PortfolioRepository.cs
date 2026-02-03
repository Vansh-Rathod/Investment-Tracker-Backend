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
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PortfolioRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<PortfolioDTO>> GetUserPortfoliosAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var result = await connection.QueryAsync<PortfolioDTO>(
                "GetUserPortfolios",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<PortfolioDTO?> GetPortfolioByIdAsync(int portfolioId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var portfolios = await connection.QueryAsync<PortfolioDTO>(
                "GetUserPortfolios",
                parameters,
                commandType: CommandType.StoredProcedure);

            return portfolios.FirstOrDefault(p => p.PortfolioId == portfolioId);
        }

        public async Task<int> CreatePortfolioAsync(int userId, CreatePortfolioRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PortfolioId", null);
            parameters.Add("@Name", request.Name);
            parameters.Add("@UserId", userId);
            parameters.Add("@PortfolioType", request.PortfolioType);
            parameters.Add("@Operation", "INSERT");

            var portfolioId = await connection.ExecuteScalarAsync<int>(
                "InsertUpdateDeletePortfolio",
                parameters,
                commandType: CommandType.StoredProcedure);

            return portfolioId;
        }

        public async Task<bool> UpdatePortfolioAsync(int userId, UpdatePortfolioRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PortfolioId", request.PortfolioId);
            parameters.Add("@Name", request.Name);
            parameters.Add("@UserId", userId);
            parameters.Add("@PortfolioType", request.PortfolioType);
            parameters.Add("@Operation", "UPDATE");

            var result = await connection.ExecuteAsync(
                "InsertUpdateDeletePortfolio",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> DeletePortfolioAsync(int portfolioId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PortfolioId", portfolioId);
            parameters.Add("@Name", null);
            parameters.Add("@UserId", userId);
            parameters.Add("@PortfolioType", null);
            parameters.Add("@Operation", "DELETE");

            var result = await connection.ExecuteAsync(
                "InsertUpdateDeletePortfolio",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result > 0;
        }
    }
}
