using System;

namespace Core.DTOs
{
    public class CreateTransactionRequest
    {
        public int PortfolioId { get; set; }
        public int AssetTypeId { get; set; }
        public int AssetId { get; set; }
        public int TransactionType { get; set; } // 1=Buy, 2=Sell, 3=Dividend, 4=Split, 5=Bonus
        public decimal Units { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string SourceType { get; set; } // "SIP", "Manual"
        public int? SourceId { get; set; } // SipId if from SIP
    }
}
