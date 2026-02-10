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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public CategoryRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<List<CategoryViewModel>>> GetCategories(int categoryId = 0, int categoryType = 0, string categoryName = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@CategoryId", categoryId);
                parameters.Add("@CategoryType", categoryType);
                parameters.Add("@CategoryName", categoryName);

                var data = await connection.QueryAsync<CategoryViewModel>(
                    "GetCategories",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<List<CategoryViewModel>>.SuccessDbResponse(
                    data.ToList(),
                    "Categories fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch Categories", Core.Enums.Enum.LogLevel.Error, "CategoryRepository.GetCategories", ex, new Dictionary<string, object>
                {
                    { "CategoryId", categoryId },
                    { "CategoryType", categoryType },
                    { "CategoryName", categoryName }
                });
                return DbResponse<List<CategoryViewModel>>.FailureDbResponse(
                    new List<CategoryViewModel>(),
                    new List<string> { "Failed to fetch Categories." },
                    "Exception occurred while fetching Categories"
                );
            }
        }
    }
}
