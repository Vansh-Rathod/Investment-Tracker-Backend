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
    public class AssetTypeRepository : IAssetTypeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public AssetTypeRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<List<AssetTypeViewModel>>> GetAssetTypes(int assetTypeId = 0, string assetName = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@AssetTypeId", assetTypeId);
                parameters.Add("@AssetName", assetName);

                var data = await connection.QueryAsync<AssetTypeViewModel>(
                    "GetAssetTypes",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<AssetTypeViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Asset Types fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Asset Types", Core.Enums.Enum.LogLevel.Error, "AssetTypeRepository.GetAssetTypes", ex.Message, new Dictionary<string, object>
                {
                    { "AssetTypeId", assetTypeId },
                    { "AssetName", assetName }
                });
                return DbResponse<List<AssetTypeViewModel>>.FailureDbResponse(
                    new List<AssetTypeViewModel>(),
                    new List<string> { "Failed to fetch Asset Types." },
                    "Exception occurred while fetching Asset Types"
                );
            }
        }
    }
}
