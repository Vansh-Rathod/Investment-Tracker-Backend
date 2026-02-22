using Core.DTOs;
using Core.ViewModels;
using Core.CommonModels;
using Dapper;
using GenericServices.Interfaces;
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
        private readonly ILoggingService _loggingService;

        public DashboardRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<List<DashboardSummaryViewModel>>> GetDashboardSummaryAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);

                var result = await connection.QueryFirstOrDefaultAsync<DashboardSummaryViewModel>(
                    "GetDashboardSummary",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                
                var data = result != null ? new List<DashboardSummaryViewModel> { result } : Enumerable.Empty<DashboardSummaryViewModel>();

                return DbResponse<List<DashboardSummaryViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Dashboard summary fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Dashboard summary", Core.Enums.Enum.LogLevel.Error, "DashboardRepository.GetDashboardSummaryAsync", ex.Message, new Dictionary<string, object>
                {
                    { "UserId", userId }
                });
                return DbResponse<List<DashboardSummaryViewModel>>.FailureDbResponse(
                    new List<DashboardSummaryViewModel>(),
                    new List<string> { "Failed to fetch Dashboard summary." },
                    "Exception occurred while fetching Dashboard summary"
                );
            }
        }

        public async Task<DbResponse<List<AllocationDataViewModel>>> GetAssetAllocationAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);

                var data = await connection.QueryAsync<AllocationDataViewModel>(
                    "GetAssetAllocation",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<AllocationDataViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Asset allocation fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Asset allocation", Core.Enums.Enum.LogLevel.Error, "DashboardRepository.GetAssetAllocationAsync", ex.Message, new Dictionary<string, object>
                {
                    { "UserId", userId }
                });
                return DbResponse<List<AllocationDataViewModel>>.FailureDbResponse(
                    new List<AllocationDataViewModel>(),
                    new List<string> { "Failed to fetch Asset allocation." },
                    "Exception occurred while fetching Asset allocation"
                );
            }
        }

        public async Task<DbResponse<List<AllocationDataViewModel>>> GetSectorAllocationAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);

                var data = await connection.QueryAsync<AllocationDataViewModel>(
                    "GetSectorAllocation",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<AllocationDataViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Sector allocation fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Sector allocation", Core.Enums.Enum.LogLevel.Error, "DashboardRepository.GetSectorAllocationAsync", ex.Message, new Dictionary<string, object>
                {
                    { "UserId", userId }
                });
                return DbResponse<List<AllocationDataViewModel>>.FailureDbResponse(
                    new List<AllocationDataViewModel>(),
                    new List<string> { "Failed to fetch Sector allocation." },
                    "Exception occurred while fetching Sector allocation"
                );
            }
        }

        public async Task<DbResponse<List<AllocationDataViewModel>>> GetCategoryAllocationAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);

                var data = await connection.QueryAsync<AllocationDataViewModel>(
                    "GetCategoryAllocation",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<AllocationDataViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Category allocation fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Category allocation", Core.Enums.Enum.LogLevel.Error, "DashboardRepository.GetCategoryAllocationAsync", ex.Message, new Dictionary<string, object>
                {
                    { "UserId", userId }
                });
                return DbResponse<List<AllocationDataViewModel>>.FailureDbResponse(
                    new List<AllocationDataViewModel>(),
                    new List<string> { "Failed to fetch Category allocation." },
                    "Exception occurred while fetching Category allocation"
                );
            }
        }

        public async Task<DbResponse<List<AllocationDataViewModel>>> GetAMCAllocationAsync(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);

                var data = await connection.QueryAsync<AllocationDataViewModel>(
                    "GetAMCAllocation",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<AllocationDataViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "AMC allocation fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch AMC allocation", Core.Enums.Enum.LogLevel.Error, "DashboardRepository.GetAMCAllocationAsync", ex.Message, new Dictionary<string, object>
                {
                    { "UserId", userId }
                });
                return DbResponse<List<AllocationDataViewModel>>.FailureDbResponse(
                    new List<AllocationDataViewModel>(),
                    new List<string> { "Failed to fetch AMC allocation." },
                    "Exception occurred while fetching AMC allocation"
                );
            }
        }

        public async Task<DbResponse<List<PerformanceDataViewModel>>> GetPortfolioPerformanceAsync(int userId, int months = 6)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@Months", months);

                var data = await connection.QueryAsync<PerformanceDataViewModel>(
                    "GetPortfolioPerformance",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<PerformanceDataViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Portfolio performance fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Portfolio performance", Core.Enums.Enum.LogLevel.Error, "DashboardRepository.GetPortfolioPerformanceAsync", ex.Message, new Dictionary<string, object>
                {
                    { "UserId", userId },
                    { "Months", months }
                });
                return DbResponse<List<PerformanceDataViewModel>>.FailureDbResponse(
                    new List<PerformanceDataViewModel>(),
                    new List<string> { "Failed to fetch Portfolio performance." },
                    "Exception occurred while fetching Portfolio performance"
                );
            }
        }
    }
}
