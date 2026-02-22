using System;

namespace Core.Entities
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public int AssetTypeId { get; set; } // 1 = Stock, 2 = Mutual Fund
        public int AssetId { get; set; } // StockId or FundId
        public int TransactionType { get; set; } // 1 = BUY, 2 = SELL, 3 = DIVIDEND, 4 = SPLIT, 5 = BONUS
        public decimal Units { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? SourceType { get; set; } // e.g., "SIP", "Manual"
        public int? SourceId { get; set; } // SipId if from SIP
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
