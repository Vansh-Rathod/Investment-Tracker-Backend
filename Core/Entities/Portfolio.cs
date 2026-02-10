using System;

namespace Core.Entities
{
    public class Portfolio
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int PortfolioType { get; set; }
    }
}
