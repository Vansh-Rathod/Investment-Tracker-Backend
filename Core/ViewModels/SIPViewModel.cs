using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class SIPViewModel
    {
        public int SipId { get; set; }
        public int PortfolioId { get; set; }
        public string PortfolioName { get; set; }
        public int PortfolioType { get; set; }
        public string PortfolioTypeName { get; set; }
        public int AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public int AssetId { get; set; }
        public string AssetName { get; set; }
        //public int AMCId { get; set; }
        public string AMCName { get; set; }
        public string CategoryName { get; set; }
        public decimal SipAmount { get; set; }
        public int Frequency { get; set; } // 1 = Daily, 2 = Weekly, 3 = Monthly
        public DateTime SipDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int SipStatus { get; set; } // 1 = Active, 2 = Paused, 3 = Cancelled
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
