using Core.CommonModels;
using Core.DTOs;
using Core.ViewModels;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquityTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly ILoggingService _loggingService;

        public DashboardController(IDashboardRepository dashboardRepository, ILoggingService loggingService)
        {
            _dashboardRepository = dashboardRepository;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get dashboard summary with overall portfolio metrics
        /// </summary>
        [HttpGet("summary")]
        public async Task<APIResponse<DashboardSummaryViewModel>> GetSummary()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<DashboardSummaryViewModel>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var summary = await _dashboardRepository.GetDashboardSummaryAsync(userId);

                return APIResponse<DashboardSummaryViewModel>.SuccessResponse(
                    summary.Data.FirstOrDefault(),
                    "Dashboard summary fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching dashboard summary",
                    Core.Enums.Enum.LogLevel.Error,
                    "DashboardController.GetSummary",
                    ex.Message,
                    null
                );

                return APIResponse<DashboardSummaryViewModel>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching dashboard summary"
                );
            }
        }

        /// <summary>
        /// Get asset allocation (Stocks vs Mutual Funds)
        /// </summary>
        [HttpGet("allocation")]
        public async Task<APIResponse<List<AllocationDataViewModel>>> GetAssetAllocation()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<AllocationDataViewModel>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var allocation = await _dashboardRepository.GetAssetAllocationAsync(userId);

                return APIResponse<List<AllocationDataViewModel>>.SuccessResponse(
                    allocation.Data,
                    "Asset allocation fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching asset allocation",
                    Core.Enums.Enum.LogLevel.Error,
                    "DashboardController.GetAssetAllocation",
                    ex.Message,
                    null
                );

                return APIResponse<List<AllocationDataViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching asset allocation"
                );
            }
        }

        /// <summary>
        /// Get sector-wise allocation for stocks
        /// </summary>
        [HttpGet("stocks/sector-allocation")]
        public async Task<APIResponse<List<AllocationDataViewModel>>> GetSectorAllocation()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<AllocationDataViewModel>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var allocation = await _dashboardRepository.GetSectorAllocationAsync(userId);

                return APIResponse<List<AllocationDataViewModel>>.SuccessResponse(
                    allocation.Data,
                    "Sector allocation fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching sector allocation",
                    Core.Enums.Enum.LogLevel.Error,
                    "DashboardController.GetSectorAllocation",
                    ex.Message,
                    null
                );

                return APIResponse<List<AllocationDataViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching sector allocation"
                );
            }
        }

        /// <summary>
        /// Get category-wise allocation for mutual funds
        /// </summary>
        [HttpGet("mutual-funds/category-allocation")]
        public async Task<APIResponse<List<AllocationDataViewModel>>> GetCategoryAllocation()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<AllocationDataViewModel>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var allocation = await _dashboardRepository.GetCategoryAllocationAsync(userId);

                return APIResponse<List<AllocationDataViewModel>>.SuccessResponse(
                    allocation.Data,
                    "Category allocation fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching category allocation",
                    Core.Enums.Enum.LogLevel.Error,
                    "DashboardController.GetCategoryAllocation",
                    ex.Message,
                    null
                );

                return APIResponse<List<AllocationDataViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching category allocation"
                );
            }
        }

        /// <summary>
        /// Get AMC-wise allocation for mutual funds
        /// </summary>
        [HttpGet("mutual-funds/amc-allocation")]
        public async Task<APIResponse<List<AllocationDataViewModel>>> GetAMCAllocation()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<AllocationDataViewModel>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var allocation = await _dashboardRepository.GetAMCAllocationAsync(userId);

                return APIResponse<List<AllocationDataViewModel>>.SuccessResponse(
                    allocation.Data,
                    "AMC allocation fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching AMC allocation",
                    Core.Enums.Enum.LogLevel.Error,
                    "DashboardController.GetAMCAllocation",
                    ex.Message,
                    null
                );

                return APIResponse<List<AllocationDataViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching AMC allocation"
                );
            }
        }

        /// <summary>
        /// Get portfolio performance over time
        /// </summary>
        [HttpGet("performance")]
        public async Task<APIResponse<List<PerformanceDataViewModel>>> GetPerformance(int months = 6)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<PerformanceDataViewModel>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var performance = await _dashboardRepository.GetPortfolioPerformanceAsync(userId, months);

                return APIResponse<List<PerformanceDataViewModel>>.SuccessResponse(
                    performance.Data,
                    "Portfolio performance fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching portfolio performance",
                    Core.Enums.Enum.LogLevel.Error,
                    "DashboardController.GetPerformance",
                    ex.Message,
                    null
                );

                return APIResponse<List<PerformanceDataViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching portfolio performance"
                );
            }
        }
    }
}
