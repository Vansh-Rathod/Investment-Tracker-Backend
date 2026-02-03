using Core.CommonModels;
using Core.DTOs;
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
        public async Task<APIResponse<DashboardSummaryDTO>> GetSummary(int portfolioId = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<DashboardSummaryDTO>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var summary = await _dashboardRepository.GetDashboardSummaryAsync(userId, portfolioId);

                return APIResponse<DashboardSummaryDTO>.SuccessResponse(
                    summary,
                    "Dashboard summary fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching dashboard summary",
                    Core.Enums.Enum.LogLevel.Error,
                    "DashboardController.GetSummary",
                    ex,
                    null
                );

                return APIResponse<DashboardSummaryDTO>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching dashboard summary"
                );
            }
        }

        /// <summary>
        /// Get asset allocation (Stocks vs Mutual Funds)
        /// </summary>
        [HttpGet("allocation")]
        public async Task<APIResponse<List<AllocationDataDTO>>> GetAssetAllocation(int portfolioId = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<AllocationDataDTO>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var allocation = await _dashboardRepository.GetAssetAllocationAsync(userId, portfolioId);

                return APIResponse<List<AllocationDataDTO>>.SuccessResponse(
                    allocation.ToList(),
                    "Asset allocation fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching asset allocation",
                    Core.Enums.Enum.LogLevel.Error,
                    "DashboardController.GetAssetAllocation",
                    ex,
                    null
                );

                return APIResponse<List<AllocationDataDTO>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching asset allocation"
                );
            }
        }

        /// <summary>
        /// Get sector-wise allocation for stocks
        /// </summary>
        [HttpGet("stocks/sector-allocation")]
        public async Task<APIResponse<List<AllocationDataDTO>>> GetSectorAllocation(int portfolioId = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<AllocationDataDTO>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var allocation = await _dashboardRepository.GetSectorAllocationAsync(userId, portfolioId);

                return APIResponse<List<AllocationDataDTO>>.SuccessResponse(
                    allocation.ToList(),
                    "Sector allocation fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching sector allocation",
                    Core.Enums.Enum.LogLevel.Error,
                    "DashboardController.GetSectorAllocation",
                    ex,
                    null
                );

                return APIResponse<List<AllocationDataDTO>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching sector allocation"
                );
            }
        }

        /// <summary>
        /// Get category-wise allocation for mutual funds
        /// </summary>
        [HttpGet("mutual-funds/category-allocation")]
        public async Task<APIResponse<List<AllocationDataDTO>>> GetCategoryAllocation(int portfolioId = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<AllocationDataDTO>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var allocation = await _dashboardRepository.GetCategoryAllocationAsync(userId, portfolioId);

                return APIResponse<List<AllocationDataDTO>>.SuccessResponse(
                    allocation.ToList(),
                    "Category allocation fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching category allocation",
                    Core.Enums.Enum.LogLevel.Error,
                    "DashboardController.GetCategoryAllocation",
                    ex,
                    null
                );

                return APIResponse<List<AllocationDataDTO>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching category allocation"
                );
            }
        }

        /// <summary>
        /// Get AMC-wise allocation for mutual funds
        /// </summary>
        [HttpGet("mutual-funds/amc-allocation")]
        public async Task<APIResponse<List<AllocationDataDTO>>> GetAMCAllocation(int portfolioId = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<AllocationDataDTO>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var allocation = await _dashboardRepository.GetAMCAllocationAsync(userId, portfolioId);

                return APIResponse<List<AllocationDataDTO>>.SuccessResponse(
                    allocation.ToList(),
                    "AMC allocation fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching AMC allocation",
                    Core.Enums.Enum.LogLevel.Error,
                    "DashboardController.GetAMCAllocation",
                    ex,
                    null
                );

                return APIResponse<List<AllocationDataDTO>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching AMC allocation"
                );
            }
        }

        /// <summary>
        /// Get portfolio performance over time
        /// </summary>
        [HttpGet("performance")]
        public async Task<APIResponse<List<PerformanceDataDTO>>> GetPerformance(int portfolioId = 0, int months = 6)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<PerformanceDataDTO>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var performance = await _dashboardRepository.GetPortfolioPerformanceAsync(userId, portfolioId, months);

                return APIResponse<List<PerformanceDataDTO>>.SuccessResponse(
                    performance.ToList(),
                    "Portfolio performance fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching portfolio performance",
                    Core.Enums.Enum.LogLevel.Error,
                    "DashboardController.GetPerformance",
                    ex,
                    null
                );

                return APIResponse<List<PerformanceDataDTO>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching portfolio performance"
                );
            }
        }
    }
}
