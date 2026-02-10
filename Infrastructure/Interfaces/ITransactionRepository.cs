using Core.CommonModels;
using Core.DTOs;
using Core.Entities;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ITransactionRepository
    {
        Task<DbResponse<List<TransactionViewModel>>> GetUserTransactionsAsync(
            int userId,
            int portfolioId = 0,
            int assetId = 0,
            int assetTypeId = 0,
            int transactionType = 0,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        Task<DbResponse<int>> InsertTransaction(Transaction transaction);
    }
}
