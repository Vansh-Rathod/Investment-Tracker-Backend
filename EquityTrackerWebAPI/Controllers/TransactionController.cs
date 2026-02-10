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
        [HttpGet("transactions")]
        public async Task<APIResponse<List<TransactionViewModel>>> GetTransactions(
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
                    return APIResponse<List<TransactionViewModel>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var transactions = await _transactionRepository.GetUserTransactionsAsync(
                    userId, portfolioId, assetId, assetTypeId, transactionType, fromDate, toDate);

                return APIResponse<List<TransactionViewModel>>.SuccessResponse(
                    transactions.Data,
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

                return APIResponse<List<TransactionViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching transactions"
                );
            }
        }
    }
}
