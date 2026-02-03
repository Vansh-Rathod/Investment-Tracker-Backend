using System;

namespace Core.DTOs
{
    public class SIPDTO
    {
        public int SipId { get; set; }
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
        public decimal SipAmount { get; set; }
        public int Frequency { get; set; }
        public string FrequencyName => Frequency switch
        {
            1 => "Daily",
            2 => "Weekly",
            3 => "Monthly",
            _ => "Unknown"
        };
        public DateTime SipDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int SipStatus { get; set; }
        public string SipStatusName => SipStatus switch
        {
            1 => "Active",
            2 => "Paused",
            3 => "Cancelled",
            _ => "Unknown"
        };
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class CreateSIPRequest
    {
        public int PortfolioId { get; set; }
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
        public decimal SipAmount { get; set; }
        public int Frequency { get; set; }
        public DateTime SipDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateSIPStatusRequest
    {
        public int SipId { get; set; }
        public int Status { get; set; } // 1 = Active, 2 = Paused, 3 = Cancelled
    }
}
