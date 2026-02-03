using System;

namespace Core.DTOs
{
    public class PortfolioDTO
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int PortfolioType { get; set; }
        public string PortfolioTypeName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public decimal TotalInvested { get; set; }
        public int StockCount { get; set; }
        public int MutualFundCount { get; set; }
    }

    public class CreatePortfolioRequest
    {
        public string Name { get; set; } = string.Empty;
        public int PortfolioType { get; set; } = 1; // Default to Mixed
    }

    public class UpdatePortfolioRequest
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PortfolioType { get; set; }
    }
}
