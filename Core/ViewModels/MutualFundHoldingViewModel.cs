using System;

namespace Core.ViewModels
{
    public class MutualFundHoldingViewModel
    {
        public int FundId { get; set; }
        public string FundName { get; set; }
        public string AMCCode { get; set; }
        public string CategoryName { get; set; }
        public decimal UnitsHeld { get; set; }
        public decimal AverageNAV { get; set; }
        public decimal CurrentNAV { get; set; }
        public decimal InvestedAmount { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal AbsoluteReturn { get; set; }
        public decimal AbsoluteReturnPercentage { get; set; }
        public decimal XIRR { get; set; }
    }
}
