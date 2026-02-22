using Core.CommonModels;
using Core.DTOs;
using Core.Entities;
using Core.ViewModels;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection;
using static Core.Enums.Enum;

namespace EquityTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepository;
        private readonly ILoggingService _loggingService;

        public StockController(IStockRepository stockRepository, ILoggingService loggingService)
        {
            _stockRepository = stockRepository;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get all market stocks with pagination, search & sorting.
        /// </summary>
        [HttpGet("GetMarketStocks")]
        public async Task<APIResponse<List<StockViewModel>>> GetMarketStocks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchText = "",
            [FromQuery] string sortOrder = "ASC",
            [FromQuery] string sortField = "StockName",
            [FromQuery] bool? isEtf = null,
            [FromQuery] int exchangeId = 0)
        {
            try
            {
                var result = await _stockRepository.GetStocks(0, page, pageSize, searchText, sortField, sortOrder, isEtf, exchangeId);

                if (!result.Success)
                {
                    return APIResponse<List<StockViewModel>>.FailureResponse(
                        result.Errors,
                        result.Message ?? "No Stocks found"
                   );
                }

                if (result.Data == null || !result.Data.Any())
                {
                    return APIResponse<List<StockViewModel>>.FailureResponse(
                    new List<string> { "No Stocks found" },
                    "No Stocks found"
                );
                }

                return APIResponse<List<StockViewModel>>.SuccessResponse(
                    result.Data,
                    "Stocks fetched successfully"
                 );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching Stocks",
                    Core.Enums.Enum.LogLevel.Critical,
                    "StockController.GetMarketStocks",
                    ex.Message,
                    null
                );

                return APIResponse<List<StockViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching Stocks"
                );
            }
        }

        /// <summary>
        /// Get Stock Holdings
        /// </summary>
        [HttpGet("Holdings")]
        public async Task<APIResponse<List<UserStockHoldingViewModel>>> GetHoldings()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<UserStockHoldingViewModel>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var result = await _stockRepository.GetStockHoldings(userId);

                if (!result.Success)
                {
                    return APIResponse<List<UserStockHoldingViewModel>>.FailureResponse(
                        result.Errors,
                        result.Message ?? "No Stock Holdings found"
                   );
                }

                return APIResponse<List<UserStockHoldingViewModel>>.SuccessResponse(
                    result.Data,
                    "Stock Holdings fetched successfully"
                 );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching Stock Holdings",
                    Core.Enums.Enum.LogLevel.Critical,
                    "StockController.GetHoldings",
                    ex.Message,
                    null
                );

                return APIResponse<List<UserStockHoldingViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching Stock Holdings"
                );
            }
        }

        /// <summary>
        /// Get stock by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<APIResponse<StockViewModel>> GetStockById([FromRoute] int id)
        {
            try
            {
                var result = await _stockRepository.GetStocks(id);

                if (!result.Success)
                {
                    return APIResponse<StockViewModel>.FailureResponse(
                    result.Errors,
                    result.Message ?? $"No Stock found by id : {id}"
                );
                }

                if (result.Data == null || !result.Data.Any())
                {
                    return APIResponse<StockViewModel>.FailureResponse(
                    new List<string> { $"No Stock found by id: {id}" },
                    $"No Stock found by id: {id}"
                );
                }

                var stock = result.Data.FirstOrDefault();

                return APIResponse<StockViewModel>.SuccessResponse(
                    stock,
                    "Stock fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching Stock",
                    Core.Enums.Enum.LogLevel.Critical,
                    "StockController.GetStockById",
                    ex.Message,
                    new Dictionary<string, object> { { "StockId", id } }
                );

                return APIResponse<StockViewModel>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching the Stock"
                );
            }
        }

        /// <summary>
        /// Create a new stock
        /// </summary>
        [HttpPost("CreateStock")]
        [Authorize(Roles = "Admin")]
        public async Task<APIResponse<int>> CreateStock([FromBody] CreateStockRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Symbol))
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "Validation failed" },
                        "Stock name and symbol are required"
                    );
                }

                Stock stock = new Stock
                {
                    StockName = request.Name,
                    Symbol = request.Symbol,
                    ExchangeId = request.ExchangeId,
                    ISIN = request.ISIN,
                    IsActive = request.IsActive,
                    IsETF = request.IsETF
                };

                var result = await _stockRepository.InsertUpdateDeleteStock(stock, Core.Enums.Enum.OperationType.INSERT);

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { "Failed to create Stock" },
                    "Failed to create Stock"
                );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Stock created successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error creating Stock",
                    Core.Enums.Enum.LogLevel.Critical,
                    "StockController.CreateStock",
                    ex.Message,
                    new Dictionary<string, object> { { "Request", request } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while creating the Stock"
                );
            }
        }

        /// <summary>
        /// Update an existing stock
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<APIResponse<int>> UpdateStock([FromRoute] int id, [FromBody] UpdateStockRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Symbol))
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "Validation failed" },
                        "Stock name and symbol are required"
                    );
                }

                request.StockId = id;

                // Check if stock exists
                var existingStockResult = await _stockRepository.GetStocks(id);

                if (!existingStockResult.Success)
                {
                    return APIResponse<int>.FailureResponse(
                    existingStockResult.Errors,
                    existingStockResult.Message ?? $"No Stock found by id : {id}"
                    );
                }

                if (existingStockResult.Data == null || !existingStockResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { $"No Stock found by id: {id}" },
                    $"No Stock found by id: {id}"
                    );
                }

                Stock stock = new Stock
                {
                    StockId = id,
                    StockName = request.Name,
                    Symbol = request.Symbol,
                    ExchangeId = request.ExchangeId,
                    ISIN = request.ISIN,
                    IsActive = request.IsActive,
                    IsETF = request.IsETF
                };

                var result = await _stockRepository.InsertUpdateDeleteStock(stock, Core.Enums.Enum.OperationType.UPDATE);

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { "Failed to update Stock" },
                    "Failed to update Stock"
                );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Stock updated successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error updating Stock",
                    Core.Enums.Enum.LogLevel.Critical,
                    "StockController.UpdateStock",
                    ex.Message,
                    new Dictionary<string, object> { { "StockId", id }, { "Request", request } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while updating the Stock"
                );
            }
        }

        /// <summary>
        /// Delete a stock
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<APIResponse<int>> DeleteStock(int id)
        {
            try
            {
                var existingStockResult = await _stockRepository.GetStocks(id);

                if (!existingStockResult.Success)
                {
                    return APIResponse<int>.FailureResponse(
                    existingStockResult.Errors,
                    existingStockResult.Message ?? $"No Stock found by id : {id}"
                    );
                }

                if (existingStockResult.Data == null || !existingStockResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { $"No Stock found by id: {id}" },
                    $"No Stock found by id: {id}"
                    );
                }

                Stock stock = new Stock
                {
                    StockId = id
                };

                var result = await _stockRepository.InsertUpdateDeleteStock(stock, Core.Enums.Enum.OperationType.DELETE);

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { "Failed to delete Stock" },
                    "Failed to delete Stock"
                );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Stock deleted successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error deleting Stock",
                    Core.Enums.Enum.LogLevel.Critical,
                    "StockController.DeleteStock",
                    ex.Message,
                    new Dictionary<string, object> { { "StockId", id } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while deleting the Stock"
                );
            }
        }
    }
}
