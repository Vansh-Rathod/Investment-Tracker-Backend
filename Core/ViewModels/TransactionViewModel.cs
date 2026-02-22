using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public int AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public int AssetId { get; set; }
        public string AssetName { get; set; }
        //public int AMCId { get; set; }
        public string AMCName { get; set; }
        public string CategoryName { get; set; }
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
