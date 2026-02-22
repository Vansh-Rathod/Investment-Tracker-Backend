using Core.CommonModels;
using Core.DTOs;
using Core.Entities;
using Core.ViewModels;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [HttpGet("GetTransactions")]
        public async Task<APIResponse<List<TransactionViewModel>>> GetTransactions(
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
                    return APIResponse<List<TransactionViewModel>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var transactionsResult = await _transactionRepository.GetUserTransactionsAsync(
                    userId, assetId, assetTypeId, transactionType, fromDate, toDate);

                if (!transactionsResult.Success)
                {
                    return APIResponse<List<TransactionViewModel>>.FailureResponse(
                    transactionsResult.Errors,
                    transactionsResult.Message ?? "No Transactions found"
                );
                }

                if (transactionsResult.Data == null || !transactionsResult.Data.Any())
                {
                    return APIResponse<List<TransactionViewModel>>.FailureResponse(
                    new List<string> { "No Transactions found" },
                    "No Transactions found"
                );
                }

                return APIResponse<List<TransactionViewModel>>.SuccessResponse(
                    transactionsResult.Data,
                    "Transactions fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching transactions",
                    Core.Enums.Enum.LogLevel.Error,
                    "TransactionController.GetTransactions",
                    ex.Message,
                    null
                );

                return APIResponse<List<TransactionViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching transactions"
                );
            }
        }

        /// <summary>
        /// Get transaction by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<APIResponse<TransactionViewModel>> GetTransactionById(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<TransactionViewModel>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                // Fetch user's transactions and filter by ID
                // Ideally, the repository should support filtering by ID directly
                var transactions = await _transactionRepository.GetUserTransactionsAsync(userId);
                
                if (!transactions.Success)
                {
                    return APIResponse<TransactionViewModel>.FailureResponse(
                        transactions.Errors,
                        transactions.Message ?? "Failed to fetch transactions"
                    );
                }

                var transaction = transactions.Data?.FirstOrDefault(t => t.TransactionId == id);

                if (transaction == null)
                {
                    return APIResponse<TransactionViewModel>.FailureResponse(
                        new List<string> { "Transaction not found" },
                        $"Transaction with ID {id} not found"
                    );
                }

                return APIResponse<TransactionViewModel>.SuccessResponse(
                    transaction,
                    "Transaction fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching transaction",
                    Core.Enums.Enum.LogLevel.Error,
                    "TransactionController.GetTransactionById",
                    ex.Message,
                    new Dictionary<string, object> { { "TransactionId", id } }
                );

                return APIResponse<TransactionViewModel>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching the transaction"
                );
            }
        }

        /// <summary>
        /// Insert a new transaction
        /// </summary>
        [HttpPost("InsertTransaction")]
        public async Task<APIResponse<int>> InsertTransaction([FromBody] CreateTransactionRequest request)
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

                if (request == null)
                {
                     return APIResponse<int>.FailureResponse(
                        new List<string> { "Invalid request" },
                        "Request body is null"
                    );
                }

                Transaction transaction = new Transaction
                {
                    UserId = userId,
                    AssetTypeId = request.AssetTypeId,
                    AssetId = request.AssetId,
                    TransactionType = request.TransactionType,
                    Units = request.Units,
                    Price = request.Price,
                    Amount = request.Amount,
                    TransactionDate = request.TransactionDate,
                    SourceType = request.SourceType,
                    SourceId = request.SourceId
                };

                var result = await _transactionRepository.InsertTransaction(transaction);

                if (!result.Success)
                {
                     return APIResponse<int>.FailureResponse(
                        result.Errors,
                        result.Message ?? "Failed to insert transaction"
                    );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Transaction inserted successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error inserting transaction",
                    Core.Enums.Enum.LogLevel.Error,
                    "TransactionController.InsertTransaction",
                    ex.Message,
                    new Dictionary<string, object> { { "Request", request } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while inserting the transaction"
                );
            }
        }
    }
}
