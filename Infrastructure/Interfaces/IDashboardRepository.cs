using Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryDTO> GetDashboardSummaryAsync(int userId, int portfolioId = 0);
        Task<IEnumerable<AllocationDataDTO>> GetAssetAllocationAsync(int userId, int portfolioId = 0);
        Task<IEnumerable<AllocationDataDTO>> GetSectorAllocationAsync(int userId, int portfolioId = 0);
        Task<IEnumerable<AllocationDataDTO>> GetCategoryAllocationAsync(int userId, int portfolioId = 0);
        Task<IEnumerable<AllocationDataDTO>> GetAMCAllocationAsync(int userId, int portfolioId = 0);
        Task<IEnumerable<PerformanceDataDTO>> GetPortfolioPerformanceAsync(int userId, int portfolioId = 0, int months = 6);
    }
}
