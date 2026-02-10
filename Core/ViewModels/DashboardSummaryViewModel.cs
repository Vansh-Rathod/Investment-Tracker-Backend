using System;

namespace Core.ViewModels
{
    public class DashboardSummaryViewModel
    {
        public decimal InvestedAmount { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal TotalReturns { get; set; }
        public decimal AbsReturns { get; set; }
        public int XIRR { get; set; }
        public int DayChange { get; set; }
        public int DayChangePercentage { get; set; }
        //public decimal MutualFundsValue { get; set; }
        //public decimal StocksValue { get; set; }
        //public int MutualFundsCount { get; set; }
        //public int StocksCount { get; set; }
    }
}
