using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<TransactionDTO>> GetUserTransactionsAsync(
            int userId,
            int portfolioId = 0,
            int assetId = 0,
            int assetTypeId = 0,
            int transactionType = 0,
            DateTime? fromDate = null,
            DateTime? toDate = null);
        
        Task<IEnumerable<TransactionDTO>> GetStockTransactionsAsync(int userId, int portfolioId = 0);
        Task<IEnumerable<TransactionDTO>> GetMutualFundTransactionsAsync(int userId, int portfolioId = 0);
        Task<IEnumerable<TransactionDTO>> GetSIPTransactionsAsync(int userId, int portfolioId = 0);
    }
}
