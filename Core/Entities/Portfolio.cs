using System;

namespace Core.Entities
{
    public class Portfolio
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int PortfolioType { get; set; } // 1 = Mixed, 2 = Stocks Only, 3 = Mutual Funds Only
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
