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
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILoggingService _loggingService;

        public TransactionController(ITransactionRepository transactionRepository, ILoggingService loggingService)
        {
            _transactionRepository = transactionRepository;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get all transactions with optional filters
        /// </summary>
        [HttpGet]
        public async Task<APIResponse<List<TransactionDTO>>> GetTransactions(
            int portfolioId = 0,
            int assetId = 0,
            int assetTypeId = 0,
            int transactionType = 0,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<TransactionDTO>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var transactions = await _transactionRepository.GetUserTransactionsAsync(
                    userId, portfolioId, assetId, assetTypeId, transactionType, fromDate, toDate);

                return APIResponse<List<TransactionDTO>>.SuccessResponse(
                    transactions.ToList(),
                    "Transactions fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching transactions",
                    Core.Enums.Enum.LogLevel.Error,
                    "TransactionController.GetTransactions",
                    ex,
                    null
                );

                return APIResponse<List<TransactionDTO>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching transactions"
                );
            }
        }

        /// <summary>
        /// Get stock transactions only
        /// </summary>
        [HttpGet("stocks")]
        public async Task<APIResponse<List<TransactionDTO>>> GetStockTransactions(int portfolioId = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<TransactionDTO>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var transactions = await _transactionRepository.GetStockTransactionsAsync(userId, portfolioId);

                return APIResponse<List<TransactionDTO>>.SuccessResponse(
                    transactions.ToList(),
                    "Stock transactions fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching stock transactions",
                    Core.Enums.Enum.LogLevel.Error,
                    "TransactionController.GetStockTransactions",
                    ex,
                    null
                );

                return APIResponse<List<TransactionDTO>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching stock transactions"
                );
            }
        }

        /// <summary>
        /// Get mutual fund transactions only
        /// </summary>
        [HttpGet("mutual-funds")]
        public async Task<APIResponse<List<TransactionDTO>>> GetMutualFundTransactions(int portfolioId = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<TransactionDTO>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var transactions = await _transactionRepository.GetMutualFundTransactionsAsync(userId, portfolioId);

                return APIResponse<List<TransactionDTO>>.SuccessResponse(
                    transactions.ToList(),
                    "Mutual fund transactions fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching mutual fund transactions",
                    Core.Enums.Enum.LogLevel.Error,
                    "TransactionController.GetMutualFundTransactions",
                    ex,
                    null
                );

                return APIResponse<List<TransactionDTO>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching mutual fund transactions"
                );
            }
        }

        /// <summary>
        /// Get SIP-linked transactions only
        /// </summary>
        [HttpGet("sips")]
        public async Task<APIResponse<List<TransactionDTO>>> GetSIPTransactions(int portfolioId = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<TransactionDTO>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var transactions = await _transactionRepository.GetSIPTransactionsAsync(userId, portfolioId);

                return APIResponse<List<TransactionDTO>>.SuccessResponse(
                    transactions.ToList(),
                    "SIP transactions fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching SIP transactions",
                    Core.Enums.Enum.LogLevel.Error,
                    "TransactionController.GetSIPTransactions",
                    ex,
                    null
                );

                return APIResponse<List<TransactionDTO>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching SIP transactions"
                );
            }
        }
    }
}
