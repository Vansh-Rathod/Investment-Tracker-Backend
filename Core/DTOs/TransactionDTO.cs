using System;

namespace Core.DTOs
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public int PortfolioId { get; set; }
        public string PortfolioName { get; set; } = string.Empty;
        public int PortfolioType { get; set; }
        public string PortfolioTypeName { get; set; } = string.Empty;
        public int AssetTypeId { get; set; }
        public string AssetTypeName { get; set; } = string.Empty;
        public int AssetId { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string? AMCName { get; set; }
        public string? CategoryName { get; set; }
        public int TransactionType { get; set; }
        public string TransactionTypeName => TransactionType switch
        {
            1 => "Buy",
            2 => "Sell",
            3 => "Dividend",
            4 => "Split",
            5 => "Bonus",
            _ => "Unknown"
        };
        public decimal Units { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? SourceType { get; set; }
        public int? SourceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class TransactionFilterRequest
    {
        public int? PortfolioId { get; set; }
        public int? AssetId { get; set; }
        public int? AssetTypeId { get; set; }
        public int? TransactionType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
