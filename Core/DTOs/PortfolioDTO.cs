using System;

namespace Core.DTOs
{
    public class CreatePortfolioRequest
    {
        public string Name { get; set; } = string.Empty;
        public int PortfolioType { get; set; } = 1; // Default to Mixed
        public string Description { get; set; }
    }

    public class UpdatePortfolioRequest
    {
        public int PortfolioId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PortfolioType { get; set; }
        public string Description { get; set; }
    }
}
