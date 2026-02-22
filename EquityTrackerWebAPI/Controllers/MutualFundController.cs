using Core.CommonModels;
using Core.DTOs;
using Core.Entities;
using Core.ViewModels;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using static Core.Enums.Enum;

namespace EquityTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MutualFundController : ControllerBase
    {
        private readonly IAMCRepository _amcRepository;
        private readonly IMutualFundRepository _mutualFundRepository;
        private readonly ILoggingService _loggingService;
        private readonly ICategoryRepository _categoryRepository;
        public MutualFundController(IAMCRepository amcRepository, IMutualFundRepository mutualFundRepository, ILoggingService loggingService, ICategoryRepository categoryRepository)
        {
            _amcRepository = amcRepository;
            _mutualFundRepository = mutualFundRepository;
            _loggingService = loggingService;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Get all mutual funds with pagination, search & sorting.
        /// </summary>
        [HttpGet("GetMarketMutualFunds")]
        public async Task<APIResponse<List<MutualFundViewModel>>> GetMarketMutualFunds(
            int page = 1,
            int pageSize = 10,
            string searchText = "",
            string sortOrder = "ASC",
            string sortField = "FundName",
            int amcId = 0,
            int categoryId = 0,
            int categoryType = 0
            //bool isActive = true,
            //bool isDeleted = false
            )
        {
            var response = new APIResponse<List<MutualFundViewModel>>();
            try
            {
                var result = await _mutualFundRepository.GetMutualFunds(0, amcId, searchText, categoryId, categoryType, page, pageSize, searchText, sortField, sortOrder);

                if (!result.Success)
                {
                    return APIResponse<List<MutualFundViewModel>>.FailureResponse(
                        result.Errors,
                        result.Message ?? "No Mutual Funds found"
                   );
                }

                if (result.Data == null || !result.Data.Any())
                {
                    return APIResponse<List<MutualFundViewModel>>.FailureResponse(
                    new List<string> { "No Mutual Funds found" },
                    "No Mutual Funds found"
                );
                }

                return APIResponse<List<MutualFundViewModel>>.SuccessResponse(
                    result.Data,
                    "Mututal Funds fetched successfully"
                 );
            }
            catch(Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching Mututal Funds",
                    Core.Enums.Enum.LogLevel.Critical,
                    "MutualFundController.GetMarketMutualFunds",
                    ex.Message,
                    null
                );

                return APIResponse<List<MutualFundViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching Mututal Funds"
                );
            }
        }

        /// <summary>
        /// Get Mutual Fund Holdings
        /// </summary>
        [HttpGet("Holdings")]
        public async Task<APIResponse<List<MutualFundHoldingViewModel>>> GetHoldings()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<MutualFundHoldingViewModel>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var result = await _mutualFundRepository.GetMutualFundHoldings(userId);

                if (!result.Success)
                {
                    return APIResponse<List<MutualFundHoldingViewModel>>.FailureResponse(
                        result.Errors,
                        result.Message ?? "No Mutual Fund Holdings found"
                   );
                }

                return APIResponse<List<MutualFundHoldingViewModel>>.SuccessResponse(
                    result.Data,
                    "Mutual Fund Holdings fetched successfully"
                 );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching Mutual Fund Holdings",
                    Core.Enums.Enum.LogLevel.Critical,
                    "MutualFundController.GetHoldings",
                    ex.Message,
                    null
                );

                return APIResponse<List<MutualFundHoldingViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching Mutual Fund Holdings"
                );
            }
        }

        /// <summary>
        /// Get mutual fund by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<APIResponse<MutualFundViewModel>> GetMutualFundById([FromRoute] int id)
        {
            try
            {
                var result = await _mutualFundRepository.GetMutualFunds(id, 0, "", 0, 0, 1, 1, "", "FundName", "ASC");

                if (!result.Success)
                {
                    return APIResponse<MutualFundViewModel>.FailureResponse(
                    result.Errors,
                    result.Message ?? $"No Mutual Fund found by id : {id}"
                );
                }

                if (result.Data == null || !result.Data.Any())
                {
                    return APIResponse<MutualFundViewModel>.FailureResponse(
                    new List<string> { $"No Mutual Fund found by id: {id}" },
                    $"No Mutual Fund found by id: {id}"
                );
                }

                var mutualFund = result.Data.FirstOrDefault();

                return APIResponse<MutualFundViewModel>.SuccessResponse(
                    mutualFund,
                    "Mutual Fund fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching Mutual Fund",
                    Core.Enums.Enum.LogLevel.Critical,
                    "MutualFundController.GetMutualFundById",
                    ex.Message,
                    new Dictionary<string, object> { { "FundId", id } }
                );

                return APIResponse<MutualFundViewModel>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching the Mutual Fund"
                );
            }
        }

        /// <summary>
        /// Create a new mutual fund
        /// </summary>
        [HttpPost("CreateMutualFund")]
        [Authorize(Roles = "Admin")]
        public async Task<APIResponse<int>> CreateMutualFund([FromBody] CreateMutualFundRequest model)
        {
            try
            {
                if(model.AMCId <= 0 || string.IsNullOrWhiteSpace(model.FundName) || model.CategoryId <= 0)
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "AMCId, FundName and Category are required fields and must be valid" },
                        "Invalid input data"
                    );
                }

                var amcResult = await _amcRepository.GetAssetManagementCompanies(model.AMCId, null);

                if (!amcResult.Success)
                {
                    return APIResponse<int>.FailureResponse(
                    amcResult.Errors,
                    amcResult.Message ?? $"No Asset Management Company found by id : {model.AMCId}"
                );
                }

                if (amcResult.Data == null || !amcResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { $"No Asset Management Company found by id: {model.AMCId}" },
                    $"No Asset Management Company found by id: {model.AMCId}"
                );
                }

                var categoryResult = await _categoryRepository.GetCategories(model.CategoryId, 0, null);

                if (!categoryResult.Success)
                {
                    return APIResponse<int>.FailureResponse(
                    categoryResult.Errors,
                    categoryResult.Message ?? $"No Category found by id : {model.CategoryId}"
                );
                }

                if (categoryResult.Data == null || !categoryResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { $"No Category found by id: {model.CategoryId}" },
                    $"No Category found by id: {model.CategoryId}"
                );
                }

                MutualFund mutualFund = new MutualFund()
                {
                    AmcId = model.AMCId,
                    FundName = model.FundName,
                    Category = model.CategoryId,
                    ISIN = model.ISIN,
                    IsActive = model.IsActive
                };

                var result = await _mutualFundRepository.InsertUpdateDeleteMutualFund(
                    mutualFund,
                    OperationType.INSERT
                );

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { "Failed to create Mutual Fund" },
                    "Failed to create Mutual Fund"
                );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Mutual Fund created successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error creating mutual fund",
                    Core.Enums.Enum.LogLevel.Critical,
                    "MutualFundController.CreateMutualFund",
                    ex.Message,
                    new Dictionary<string, object> { { "Request", model } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while creating the mutual fund"
                );
            }
        }

        /// <summary>
        /// Update an existing mutual fund
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<APIResponse<int>> UpdateMutualFund([FromRoute] int id, [FromBody] UpdateMutualFundRequest model)
        {
            try
            {
                if (model.AMCId <= 0 || string.IsNullOrWhiteSpace(model.FundName) || model.CategoryId <= 0)
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "AMCId, FundName and Category are required fields and must be valid" },
                        "Invalid input data"
                    );
                }

                model.FundId = id;

                var mutualFundResult = await _mutualFundRepository.GetMutualFunds(id, 0, "", 0, 0, 1, 1, "", "FundName", "ASC");

                if (!mutualFundResult.Success)
                {
                    return APIResponse<int>.FailureResponse(
                    mutualFundResult.Errors,
                    mutualFundResult.Message ?? $"No Mutual Fund found by id : {model.FundId}"
                );
                }

                if (mutualFundResult.Data == null || !mutualFundResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { $"No Mutual Fund found by id: {model.FundId}" },
                    $"No Mutual Fund found by id: {model.FundId}"
                );
                }

                var amcResult = await _amcRepository.GetAssetManagementCompanies(model.AMCId, null);

                if (!amcResult.Success)
                {
                    return APIResponse<int>.FailureResponse(
                    amcResult.Errors,
                    amcResult.Message ?? $"No Asset Management Company found by id : {model.AMCId}"
                );
                }

                if (amcResult.Data == null || !amcResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { $"No Asset Management Company found by id: {model.AMCId}" },
                    $"No Asset Management Company found by id: {model.AMCId}"
                );
                }

                var categoryResult = await _categoryRepository.GetCategories(model.CategoryId, 0, null);

                if (!categoryResult.Success)
                {
                    return APIResponse<int>.FailureResponse(
                    categoryResult.Errors,
                    categoryResult.Message ?? $"No Category found by id : {model.CategoryId}"
                );
                }

                if (categoryResult.Data == null || !categoryResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { $"No Category found by id: {model.CategoryId}" },
                    $"No Category found by id: {model.CategoryId}"
                );
                }

                MutualFund mutualFund = new MutualFund()
                {
                    FundId = model.FundId,
                    AmcId = model.AMCId,
                    FundName = model.FundName,
                    Category = model.CategoryId,
                    ISIN = model.ISIN,
                    IsActive = model.IsActive
                };

                var result = await _mutualFundRepository.InsertUpdateDeleteMutualFund(
                    mutualFund,
                    OperationType.UPDATE
                );

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { "Failed to update Mutual Fund" },
                    "Failed to update Mutual Fund"
                );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Mutual Fund updated successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error updating mutual fund",
                    Core.Enums.Enum.LogLevel.Critical,
                    "MutualFundController.UpdateMutualFund",
                    ex.Message,
                    new Dictionary<string, object> { { "FundId", id }, { "Request", model } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while updating the mutual fund"
                );
            }
        }

        /// <summary>
        /// Delete an existing mutual fund
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<APIResponse<int>> DeleteMutualFund([FromRoute] int id)
        {
            try
            {
                var mutualFundResult = await _mutualFundRepository.GetMutualFunds(id, 0, "", 0, 0, 1, 1, "", "FundName", "ASC");

                if (!mutualFundResult.Success)
                {
                    return APIResponse<int>.FailureResponse(
                    mutualFundResult.Errors,
                    mutualFundResult.Message ?? $"No Mutual Fund found by id : {id}"
                );
                }

                if (mutualFundResult.Data == null || !mutualFundResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { $"No Mutual Fund found by id: {id}" },
                    $"No Mutual Fund found by id: {id}"
                );
                }

                MutualFund mutualFund = new MutualFund()
                {
                    FundId = id
                };

                var result = await _mutualFundRepository.InsertUpdateDeleteMutualFund(
                    mutualFund,
                    OperationType.DELETE
                );

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { "Failed to delete Mutual Fund" },
                    "Failed to delete Mutual Fund"
                );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Mutual Fund deleted successfully"
                );
            }
            catch(Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error deleting mutual fund",
                    Core.Enums.Enum.LogLevel.Critical,
                    "MutualFundController.DeleteMutualFund",
                    ex.Message,
                    new Dictionary<string, object> { { "FundId", id } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while deleting the mutual fund"
                );
            }
        }
    }
}
