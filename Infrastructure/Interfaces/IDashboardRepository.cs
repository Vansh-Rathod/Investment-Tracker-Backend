using Core.CommonModels;
using Core.DTOs;
using Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DbResponse<List<DashboardSummaryViewModel>>> GetDashboardSummaryAsync(int userId);
        Task<DbResponse<List<AllocationDataViewModel>>> GetAssetAllocationAsync(int userId);
        Task<DbResponse<List<AllocationDataViewModel>>> GetSectorAllocationAsync(int userId);
        Task<DbResponse<List<AllocationDataViewModel>>> GetCategoryAllocationAsync(int userId);
        Task<DbResponse<List<AllocationDataViewModel>>> GetAMCAllocationAsync(int userId);
        Task<DbResponse<List<PerformanceDataViewModel>>> GetPortfolioPerformanceAsync(int userId, int months = 6);
    }
}
