using Core.CommonModels;
using Dapper;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GenericServices.Interfaces;

namespace Infrastructure.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public SettingsRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<Dictionary<string, string>>> GetUserSettings(int userId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);

                var data = await connection.QueryAsync<(string Key, string Value)>(
                    "GetUserSettings",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                var dict = data.ToDictionary(x => x.Key, x => x.Value);

                return DbResponse<Dictionary<string, string>>.SuccessDbResponse(dict, "Settings fetched");
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error fetching settings", Core.Enums.Enum.LogLevel.Error, "SettingsRepository.GetUserSettings", ex.Message, new Dictionary<string, object> { { "UserId", userId } });
                return DbResponse<Dictionary<string, string>>.FailureDbResponse(new Dictionary<string, string>(), new List<string> { "Failed to fetch" }, "Error");
            }
        }

        public async Task<DbResponse<bool>> SaveUserSetting(int userId, string key, string value)
        {
             try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@KeyName", key);
                parameters.Add("@Value", value);

                await connection.ExecuteAsync(
                    "SaveUserSetting",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<bool>.SuccessDbResponse(true, "Setting saved");
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error saving setting", Core.Enums.Enum.LogLevel.Error, "SettingsRepository.SaveUserSetting", ex.Message, new Dictionary<string, object> { { "UserId", userId } });
                return DbResponse<bool>.FailureDbResponse(false, new List<string> { "Failed to save" }, "Error");
            }
        }
    }
}
