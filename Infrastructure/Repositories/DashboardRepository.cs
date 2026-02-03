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
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DashboardRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<DashboardSummaryDTO> GetDashboardSummaryAsync(int userId, int portfolioId = 0)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PortfolioId", portfolioId);

            var result = await connection.QueryFirstOrDefaultAsync<DashboardSummaryDTO>(
                "GetDashboardSummary",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result ?? new DashboardSummaryDTO();
        }

        public async Task<IEnumerable<AllocationDataDTO>> GetAssetAllocationAsync(int userId, int portfolioId = 0)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PortfolioId", portfolioId);

            var result = await connection.QueryAsync<AllocationDataDTO>(
                "GetAssetAllocation",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<IEnumerable<AllocationDataDTO>> GetSectorAllocationAsync(int userId, int portfolioId = 0)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PortfolioId", portfolioId);

            var result = await connection.QueryAsync<AllocationDataDTO>(
                "GetSectorAllocation",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<IEnumerable<AllocationDataDTO>> GetCategoryAllocationAsync(int userId, int portfolioId = 0)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PortfolioId", portfolioId);

            var result = await connection.QueryAsync<AllocationDataDTO>(
                "GetCategoryAllocation",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<IEnumerable<AllocationDataDTO>> GetAMCAllocationAsync(int userId, int portfolioId = 0)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PortfolioId", portfolioId);

            var result = await connection.QueryAsync<AllocationDataDTO>(
                "GetAMCAllocation",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<IEnumerable<PerformanceDataDTO>> GetPortfolioPerformanceAsync(int userId, int portfolioId = 0, int months = 6)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PortfolioId", portfolioId);
            parameters.Add("@Months", months);

            var result = await connection.QueryAsync<PerformanceDataDTO>(
                "GetPortfolioPerformance",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
    }
}
