using System;

namespace Core.DTOs
{
    public class CreateMutualFundRequest
    {
        public string FundName { get; set; }
        public int AmcId { get; set; }
        public int CategoryId { get; set; }
        public int RiskLevel { get; set; } // 1=Low, 2=Medium, 3=High
        public decimal ExpenseRatio { get; set; }
        public string FundManager { get; set; }
        public DateTime InceptionDate { get; set; }
        public decimal Nav { get; set; }
        public decimal Rating { get; set; }
        public string Description { get; set; }
    }

    public class UpdateMutualFundRequest : CreateMutualFundRequest
    {
        public int MutualFundId { get; set; }
        public bool IsActive { get; set; }
    }
}
