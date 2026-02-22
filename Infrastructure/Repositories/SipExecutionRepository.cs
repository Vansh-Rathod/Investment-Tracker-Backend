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

namespace Infrastructure.Repositories
{
    public class SipExecutionRepository : ISipExecutionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public SipExecutionRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<List<SipExecutionViewModel>>> GetUserSipExecutionsAsync(
            int userId,
            int sipId = 0,
            int executionStatus = 0,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@SipId", sipId);
                parameters.Add("@ExecutionStatus", executionStatus);
                parameters.Add("@FromDate", fromDate);
                parameters.Add("@ToDate", toDate);

                var data = await connection.QueryAsync<SipExecutionViewModel>(
                    "GetUserSipExecutions",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<SipExecutionViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "SIP executions fetched successfully");
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Failed to fetch SIP executions",
                    Core.Enums.Enum.LogLevel.Error,
                    "SipExecutionRepository.GetUserSipExecutionsAsync",
                    ex.Message,
                    new Dictionary<string, object>
                    {
                        { "UserId", userId },
                        { "SipId", sipId },
                        { "ExecutionStatus", executionStatus }
                    });
                return DbResponse<List<SipExecutionViewModel>>.FailureDbResponse(
                    new List<SipExecutionViewModel>(),
                    new List<string> { "Failed to fetch SIP executions." },
                    "Exception occurred while fetching SIP executions");
            }
        }
    }
}
