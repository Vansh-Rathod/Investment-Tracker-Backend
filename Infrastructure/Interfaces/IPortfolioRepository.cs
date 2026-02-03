using Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<IEnumerable<PortfolioDTO>> GetUserPortfoliosAsync(int userId);
        Task<PortfolioDTO?> GetPortfolioByIdAsync(int portfolioId, int userId);
        Task<int> CreatePortfolioAsync(int userId, CreatePortfolioRequest request);
        Task<bool> UpdatePortfolioAsync(int userId, UpdatePortfolioRequest request);
        Task<bool> DeletePortfolioAsync(int portfolioId, int userId);
    }
}
