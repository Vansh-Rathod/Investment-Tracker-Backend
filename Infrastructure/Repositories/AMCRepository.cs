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
    public class AMCRepository : IAMCRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public AMCRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<List<AMCViewModel>>> GetAssetManagementCompanies(int amcId = 0, string amcName = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@AssetManagementCompanyId", amcId);
                parameters.Add("@AssetManagementCompanyName", amcName);

                var data = await connection.QueryAsync<AMCViewModel>(
                    "GetAssetManagementCompanies",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<AMCViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Asset Management Companies fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch AMCs", Core.Enums.Enum.LogLevel.Error, "AMCRepository.GetAssetManagementCompanies", ex.Message, new Dictionary<string, object>
                {
                    { "AMCId", amcId },
                    { "AMCName", amcName }
                });
                return DbResponse<List<AMCViewModel>>.FailureDbResponse(
                    new List<AMCViewModel>(),
                    new List<string> { "Failed to fetch Asset Management Companies." },
                    "Exception occurred while fetching AMCs"
                );
            }
        }

        public async Task<DbResponse<int>> InsertUpdateDeleteAMC(AMC amc, OperationType operationType)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@OperationType", (int)operationType);
                parameters.Add("@AmcId", amc.AmcId);
                parameters.Add("@Name", amc.Name);

                var resultId = await connection.ExecuteScalarAsync<int>(
                    "InsertUpdateDeleteAMC",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<int>.SuccessDbResponse(
                    resultId,
                    $"AMC {operationType} operation successful"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to perform AMC operation", Core.Enums.Enum.LogLevel.Error, "AMCRepository.InsertUpdateDeleteAMC", ex.Message, new Dictionary<string, object>
                {
                    { "OperationType", operationType },
                    { "AmcId", amc?.AmcId },
                    { "Name", amc?.Name }
                });
                return DbResponse<int>.FailureDbResponse(0, new List<string> { "Failed to perform AMC operation." }, "Exception occurred");
            }
        }
    }
}
