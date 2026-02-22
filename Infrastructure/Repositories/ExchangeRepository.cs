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
    public class ExchangeRepository : IExchangeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public ExchangeRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<List<ExchangeViewModel>>> GetExchanges(int exchangeId = 0, string exchangeName = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@ExchangeId", exchangeId);
                parameters.Add("@ExchangeName", exchangeName);

                var data = await connection.QueryAsync<ExchangeViewModel>(
                    "GetExchanges",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<ExchangeViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Exchanges fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Exchanges", Core.Enums.Enum.LogLevel.Error, "ExchangeRepository.GetExchanges", ex.Message, new Dictionary<string, object>
                {
                    { "ExchangeId", exchangeId },
                    { "ExchangeName", exchangeName }
                });
                return DbResponse<List<ExchangeViewModel>>.FailureDbResponse(
                    new List<ExchangeViewModel>(),
                    new List<string> { "Failed to fetch Exchanges." },
                    "Exception occurred while fetching Exchanges"
                );
            }
        }
    }
}
