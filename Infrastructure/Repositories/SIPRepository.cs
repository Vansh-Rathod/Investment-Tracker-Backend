using Core.DTOs;
using Core.CommonModels;
using Dapper;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.ViewModels;

using Core.Entities;
using static Core.Enums.Enum;

namespace Infrastructure.Repositories
{
    public class SIPRepository : ISIPRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public SIPRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<List<SIPViewModel>>> GetUserSIPsAsync(int userId, int sipId = 0, int portfolioId = 0, int sipStatus = 0, int portfolioType = 0)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@SipId", sipId);
                parameters.Add("@PortfolioId", portfolioId);
                parameters.Add("@SipStatus", sipStatus);
                parameters.Add("@PortfolioType", portfolioType);

                var data = await connection.QueryAsync<SIPViewModel>(
                    "GetUserSIPs",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<SIPViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "SIPs fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch SIPs", Core.Enums.Enum.LogLevel.Error, "SIPRepository.GetUserSIPsAsync", ex, new Dictionary<string, object>
                {
                    { "UserId", userId },
                    { "SipId", sipId },
                    { "PortfolioId", portfolioId },
                    { "SipStatus", sipStatus },
                    { "PortfolioType", portfolioType }
                });
                return DbResponse<List<SIPViewModel>>.FailureDbResponse(
                    new List<SIPViewModel>(),
                    new List<string> { "Failed to fetch SIPs." },
                    "Exception occurred while fetching SIPs"
                );
            }
        }

        public async Task<DbResponse<int>> InsertUpdateDeleteSIP(SIP sip, OperationType operationType)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@OperationType", (int)operationType);
                parameters.Add("@SipId", sip.SipId);
                parameters.Add("@PortfolioId", sip.PortfolioId);
                parameters.Add("@AssetTypeId", sip.AssetTypeId);
                parameters.Add("@AssetId", sip.AssetId);
                parameters.Add("@SipAmount", sip.SipAmount);
                parameters.Add("@Frequency", sip.Frequency);
                parameters.Add("@SipDate", sip.SipDate);
                parameters.Add("@StartDate", sip.StartDate);
                parameters.Add("@EndDate", sip.EndDate);
                parameters.Add("@Status", sip.Status);

                var resultId = await connection.ExecuteScalarAsync<int>(
                    "InsertUpdateDeleteSIP",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<int>.SuccessDbResponse(
                    resultId,
                    $"SIP {operationType} operation successful"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to perform SIP operation", Core.Enums.Enum.LogLevel.Error, "SIPRepository.InsertUpdateDeleteSIP", ex, new Dictionary<string, object>
                {
                    { "OperationType", operationType },
                    { "SipId", sip?.SipId },
                    { "PortfolioId", sip?.PortfolioId }
                });
                return DbResponse<int>.FailureDbResponse(0, new List<string> { "Failed to perform SIP operation." }, "Exception occurred");
            }
        }

    }
}
