using System;

namespace Core.Entities
{
    public class SIP
    {
        public int SipId { get; set; }
        public int UserId { get; set; }
        public int AssetTypeId { get; set; } // 1 = Stock, 2 = Mutual Fund
        public int AssetId { get; set; } // StockId or FundId
        public decimal SipAmount { get; set; }
        public int Frequency { get; set; } // 1 = Daily, 2 = Weekly, 3 = Monthly
        public DateTime SipDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Status { get; set; } // 1 = Active, 2 = Paused, 3 = Cancelled
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
