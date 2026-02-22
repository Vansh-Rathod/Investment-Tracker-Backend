using System;

namespace Core.DTOs
{
    public class CreateSIPRequest
    {
        public int AssetTypeId { get; set; }
        public int AssetId { get; set; }
        public decimal SipAmount { get; set; }
        public int Frequency { get; set; } = 3; // Default to Monthly
        public DateTime SipDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateSIPRequest
    {
        public int SipId { get; set; }
        public int AssetTypeId { get; set; } 
        public int AssetId { get; set; }
        public decimal SipAmount { get; set; }
        public int Frequency { get; set; }
        public DateTime SipDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Status { get; set; }
    }
}
