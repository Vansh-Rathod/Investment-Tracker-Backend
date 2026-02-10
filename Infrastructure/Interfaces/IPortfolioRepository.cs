using Core.CommonModels;
using Core.DTOs;
using Core.Entities;
using Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Core.Enums.Enum;

namespace Infrastructure.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<DbResponse<List<PortfolioViewModel>>> GetUserPortfoliosAsync(int userId, int portfolioId = 0, int portfolioType = 0);
        Task<DbResponse<int>> InsertUpdateDeletePortfolio(Portfolio portfolio, OperationType operationType);
    }
}
