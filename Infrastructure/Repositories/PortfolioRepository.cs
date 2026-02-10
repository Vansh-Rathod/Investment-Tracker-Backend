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
using static System.Runtime.InteropServices.JavaScript.JSType;

using Core.Entities;
using static Core.Enums.Enum;

namespace Infrastructure.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public PortfolioRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<List<PortfolioViewModel>>> GetUserPortfoliosAsync(int userId, int portfolioId = 0, int portfolioType = 0)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@PortfolioId", portfolioId);
                parameters.Add("@UserId", userId);
                parameters.Add("@PortfolioType", portfolioType);

                var data = await connection.QueryAsync<PortfolioViewModel>(
                    "GetUserPortfolios",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<PortfolioViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Portfolios fetched successfully"
                );
            }
            catch(Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Portfolios", Core.Enums.Enum.LogLevel.Error, "PortfolioRepository.GetUserPortfoliosAsync", ex, new Dictionary<string, object>
                {
                    { "UserId", userId },
                    { "PortfolioId", portfolioId },
                    { "PortfolioType", portfolioType }
                });
                return DbResponse<List<PortfolioViewModel>>.FailureDbResponse(
                    new List<PortfolioViewModel>(),
                    new List<string> { "Failed to fetch Portfolios." },
                    "Exception occurred while fetching Portfolios"
                );
            }
        }

        public async Task<DbResponse<int>> InsertUpdateDeletePortfolio(Portfolio portfolio, OperationType operationType)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@OperationType", (int)operationType);
                parameters.Add("@PortfolioId", portfolio.PortfolioId);
                parameters.Add("@Name", portfolio.Name);
                parameters.Add("@UserId", portfolio.UserId);
                parameters.Add("@PortfolioType", portfolio.PortfolioType);

                var resultId = await connection.ExecuteScalarAsync<int>(
                    "InsertUpdateDeletePortfolio",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<int>.SuccessDbResponse(
                    resultId,
                    $"Portfolio {operationType} operation successful"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to perform Portfolio operation", Core.Enums.Enum.LogLevel.Error, "PortfolioRepository.InsertUpdateDeletePortfolio", ex, new Dictionary<string, object>
                {
                    { "OperationType", operationType },
                    { "PortfolioId", portfolio?.PortfolioId },
                    { "Name", portfolio?.Name }
                });
                return DbResponse<int>.FailureDbResponse(0, new List<string> { "Failed to perform Portfolio operation." }, "Exception occurred");
            }
        }

    }
}
