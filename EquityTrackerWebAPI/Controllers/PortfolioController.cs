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
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ILoggingService _loggingService;

        public PortfolioController(IPortfolioRepository portfolioRepository, ILoggingService loggingService)
        {
            _portfolioRepository = portfolioRepository;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get all portfolios for the authenticated user
        /// </summary>
        [HttpGet]
        public async Task<APIResponse<List<PortfolioDTO>>> GetPortfolios()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<PortfolioDTO>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var portfolios = await _portfolioRepository.GetUserPortfoliosAsync(userId);

                return APIResponse<List<PortfolioDTO>>.SuccessResponse(
                    portfolios.ToList(),
                    "Portfolios fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching portfolios",
                    Core.Enums.Enum.LogLevel.Error,
                    "PortfolioController.GetPortfolios",
                    ex,
                    null
                );

                return APIResponse<List<PortfolioDTO>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching portfolios"
                );
            }
        }

        /// <summary>
        /// Get a specific portfolio by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<APIResponse<PortfolioDTO>> GetPortfolio(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<PortfolioDTO>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var portfolio = await _portfolioRepository.GetPortfolioByIdAsync(id, userId);

                if (portfolio == null)
                {
                    return APIResponse<PortfolioDTO>.FailureResponse(
                        new List<string> { "Portfolio not found" },
                        $"Portfolio with ID {id} not found"
                    );
                }

                return APIResponse<PortfolioDTO>.SuccessResponse(
                    portfolio,
                    "Portfolio fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching portfolio",
                    Core.Enums.Enum.LogLevel.Error,
                    "PortfolioController.GetPortfolio",
                    ex,
                    new Dictionary<string, object> { { "PortfolioId", id } }
                );

                return APIResponse<PortfolioDTO>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching the portfolio"
                );
            }
        }

        /// <summary>
        /// Create a new portfolio
        /// </summary>
        [HttpPost]
        public async Task<APIResponse<int>> CreatePortfolio([FromBody] CreatePortfolioRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "Validation failed" },
                        "Portfolio name is required"
                    );
                }

                var portfolioId = await _portfolioRepository.CreatePortfolioAsync(userId, request);

                return APIResponse<int>.SuccessResponse(
                    portfolioId,
                    "Portfolio created successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error creating portfolio",
                    Core.Enums.Enum.LogLevel.Error,
                    "PortfolioController.CreatePortfolio",
                    ex,
                    new Dictionary<string, object> { { "Request", request } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while creating the portfolio"
                );
            }
        }

        /// <summary>
        /// Update an existing portfolio
        /// </summary>
        [HttpPut("{id}")]
        public async Task<APIResponse<bool>> UpdatePortfolio(int id, [FromBody] UpdatePortfolioRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Validation failed" },
                        "Portfolio name is required"
                    );
                }

                request.PortfolioId = id;
                var success = await _portfolioRepository.UpdatePortfolioAsync(userId, request);

                if (!success)
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Update failed" },
                        "Failed to update portfolio"
                    );
                }

                return APIResponse<bool>.SuccessResponse(
                    true,
                    "Portfolio updated successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error updating portfolio",
                    Core.Enums.Enum.LogLevel.Error,
                    "PortfolioController.UpdatePortfolio",
                    ex,
                    new Dictionary<string, object> { { "PortfolioId", id }, { "Request", request } }
                );

                return APIResponse<bool>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while updating the portfolio"
                );
            }
        }

        /// <summary>
        /// Delete a portfolio
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<APIResponse<bool>> DeletePortfolio(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var success = await _portfolioRepository.DeletePortfolioAsync(id, userId);

                if (!success)
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Delete failed" },
                        "Failed to delete portfolio. It may have existing transactions or SIPs."
                    );
                }

                return APIResponse<bool>.SuccessResponse(
                    true,
                    "Portfolio deleted successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error deleting portfolio",
                    Core.Enums.Enum.LogLevel.Error,
                    "PortfolioController.DeletePortfolio",
                    ex,
                    new Dictionary<string, object> { { "PortfolioId", id } }
                );

                return APIResponse<bool>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while deleting the portfolio"
                );
            }
        }
    }
}
