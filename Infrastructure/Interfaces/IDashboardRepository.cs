using Core.CommonModels;
using Core.DTOs;
using Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DbResponse<List<DashboardSummaryViewModel>>> GetDashboardSummaryAsync(int userId, int portfolioId = 0);
        Task<DbResponse<List<AllocationDataViewModel>>> GetAssetAllocationAsync(int userId, int portfolioId = 0);
        Task<DbResponse<List<AllocationDataViewModel>>> GetSectorAllocationAsync(int userId, int portfolioId = 0);
        Task<DbResponse<List<AllocationDataViewModel>>> GetCategoryAllocationAsync(int userId, int portfolioId = 0);
        Task<DbResponse<List<AllocationDataViewModel>>> GetAMCAllocationAsync(int userId, int portfolioId = 0);
        Task<DbResponse<List<PerformanceDataViewModel>>> GetPortfolioPerformanceAsync(int userId, int portfolioId = 0, int months = 6);
    }
}
