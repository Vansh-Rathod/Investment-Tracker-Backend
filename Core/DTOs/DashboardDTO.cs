using System.Collections.Generic;

namespace Core.DTOs
{
    public class DashboardSummaryDTO
    {
        public decimal TotalInvested { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal TotalReturns { get; set; }
        public decimal TotalReturnsPercentage { get; set; }
        public decimal MutualFundsValue { get; set; }
        public decimal StocksValue { get; set; }
        public int MutualFundsCount { get; set; }
        public int StocksCount { get; set; }
    }

    public class AllocationDataDTO
    {
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public decimal Percentage { get; set; }
    }

    public class PerformanceDataDTO
    {
        public string Period { get; set; } = string.Empty; // e.g., "Jan 2024", "Week 1"
        public decimal MutualFunds { get; set; }
        public decimal Stocks { get; set; }
        public decimal Total { get; set; }
    }

    public class DashboardResponse
    {
        public DashboardSummaryDTO Summary { get; set; } = new();
        public List<AllocationDataDTO> AssetAllocation { get; set; } = new();
        public List<AllocationDataDTO> SectorAllocation { get; set; } = new();
        public List<AllocationDataDTO> CategoryAllocation { get; set; } = new();
        public List<AllocationDataDTO> AMCAllocation { get; set; } = new();
        public List<PerformanceDataDTO> PerformanceData { get; set; } = new();
    }
}
