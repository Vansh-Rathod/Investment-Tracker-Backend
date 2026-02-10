using System;

namespace Core.ViewModels
{
    public class PerformanceDataViewModel
    {
        //public string Period { get; set; } = string.Empty; // e.g., "Jan 2024", "Week 1"
        //public decimal MutualFunds { get; set; }
        //public decimal Stocks { get; set; }
        //public decimal Total { get; set; }

        public DateTime Date { get; set; }
        public decimal InvestedAmount { get; set; }
        public decimal CurrentValue { get; set; }
    }
}
