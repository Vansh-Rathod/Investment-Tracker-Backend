using Core.CommonModels;
using Core.DTOs;
using Core.Entities;
using Core.ViewModels;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
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
        [HttpGet("GetUserPortfolios")]
        public async Task<APIResponse<List<PortfolioViewModel>>> GetUserPortfolios()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<PortfolioViewModel>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var portfoliosResult = await _portfolioRepository.GetUserPortfoliosAsync(userId);

                if (!portfoliosResult.Success)
                {
                    return APIResponse<List<PortfolioViewModel>>.FailureResponse(
                    portfoliosResult.Errors,
                    portfoliosResult.Message ?? "No Portfolio found"
                );
                }

                if (portfoliosResult.Data == null || !portfoliosResult.Data.Any())
                {
                    return APIResponse<List<PortfolioViewModel>>.FailureResponse(
                    new List<string> { "No Portfolio found" },
                    "No Portfolio found"
                );
                }

                return APIResponse<List<PortfolioViewModel>>.SuccessResponse(
                    portfoliosResult.Data,
                    "Portfolios fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching portfolios",
                    Core.Enums.Enum.LogLevel.Error,
                    "PortfolioController.GetUserPortfolios",
                    ex.Message,
                    null
                );

                return APIResponse<List<PortfolioViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching portfolios"
                );
            }
        }

        /// <summary>
        /// Get a specific portfolio by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<APIResponse<PortfolioViewModel>> GetPortfolioById([FromRoute] int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<PortfolioViewModel>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var portfoliosResult = await _portfolioRepository.GetUserPortfoliosAsync(userId, id);

                if (!portfoliosResult.Success)
                {
                    return APIResponse<PortfolioViewModel>.FailureResponse(
                    portfoliosResult.Errors,
                    portfoliosResult.Message ?? $"No Portfolio found by id: {id}"
                );
                }

                if (portfoliosResult.Data == null || !portfoliosResult.Data.Any())
                {
                    return APIResponse<PortfolioViewModel>.FailureResponse(
                    new List<string> { $"No Portfolio found by id: {id}" },
                    $"No Portfolio found by id: {id}"
                );
                }

                return APIResponse<PortfolioViewModel>.SuccessResponse(
                    portfoliosResult.Data.FirstOrDefault(),
                    "Portfolio fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching portfolio",
                    Core.Enums.Enum.LogLevel.Error,
                    "PortfolioController.GetPortfolioById",
                    ex.Message,
                    new Dictionary<string, object> { { "PortfolioId", id } }
                );

                return APIResponse<PortfolioViewModel>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching the portfolio"
                );
            }
        }

        /// <summary>
        /// Create a new portfolio
        /// </summary>
        [HttpPost("CreatePortfolio")]
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

                Portfolio portfolio = new Portfolio
                {
                    Name = request.Name,
                    UserId = userId,
                    PortfolioType = request.PortfolioType
                };

                var result = await _portfolioRepository.InsertUpdateDeletePortfolio(portfolio, Core.Enums.Enum.OperationType.INSERT);

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "Failed to create Portfolio" },
                        "Failed to create Portfolio"
                    );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Portfolio created successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error creating portfolio",
                    Core.Enums.Enum.LogLevel.Critical,
                    "PortfolioController.CreatePortfolio",
                    ex.Message,
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
        public async Task<APIResponse<int>> UpdatePortfolio([FromRoute] int id, [FromBody] UpdatePortfolioRequest request)
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

                request.PortfolioId = id;

                // Check if portfolio exists
                var existingPortfolioResult = await _portfolioRepository.GetUserPortfoliosAsync(userId, id);

                if (!existingPortfolioResult.Success)
                {
                    return APIResponse<int>.FailureResponse(
                        existingPortfolioResult.Errors,
                        existingPortfolioResult.Message ?? $"No Portfolio found by id: {id}"
                    );
                }

                if (existingPortfolioResult.Data == null || !existingPortfolioResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { $"No Portfolio found by id: {id}" },
                        $"No Portfolio found by id: {id}"
                    );
                }

                Portfolio portfolio = new Portfolio
                {
                    PortfolioId = id,
                    Name = request.Name,
                    UserId = userId, // Ensure user owns the portfolio being updated
                    PortfolioType = request.PortfolioType
                };

                var result = await _portfolioRepository.InsertUpdateDeletePortfolio(portfolio, Core.Enums.Enum.OperationType.UPDATE);

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "Failed to update Portfolio" },
                        "Failed to update Portfolio"
                    );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Portfolio updated successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error updating portfolio",
                    Core.Enums.Enum.LogLevel.Critical,
                    "PortfolioController.UpdatePortfolio",
                    ex.Message,
                    new Dictionary<string, object> { { "PortfolioId", id }, { "Request", request } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while updating the portfolio"
                );
            }
        }

        /// <summary>
        /// Delete a portfolio
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<APIResponse<int>> DeletePortfolio(int id)
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

                // Check if portfolio exists
                var existingPortfolioResult = await _portfolioRepository.GetUserPortfoliosAsync(userId, id);

                if (!existingPortfolioResult.Success)
                {
                    return APIResponse<int>.FailureResponse(
                        existingPortfolioResult.Errors,
                        existingPortfolioResult.Message ?? $"No Portfolio found by id: {id}"
                    );
                }

                if (existingPortfolioResult.Data == null || !existingPortfolioResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { $"No Portfolio found by id: {id}" },
                        $"No Portfolio found by id: {id}"
                    );
                }

                Portfolio portfolio = new Portfolio
                {
                    PortfolioId = id,
                    UserId = userId
                };

                var result = await _portfolioRepository.InsertUpdateDeletePortfolio(portfolio, Core.Enums.Enum.OperationType.DELETE);

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "Failed to delete Portfolio" },
                        "Failed to delete Portfolio. It may have existing transactions or SIPs."
                    );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Portfolio deleted successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error deleting portfolio",
                    Core.Enums.Enum.LogLevel.Critical,
                    "PortfolioController.DeletePortfolio",
                    ex.Message,
                    new Dictionary<string, object> { { "PortfolioId", id } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while deleting the portfolio"
                );
            }
        }
    }
}
