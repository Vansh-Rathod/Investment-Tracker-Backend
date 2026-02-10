using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SipExecution
    {
        public int SipExecutionId
        {
            get; set;
        }
        public int SipId
        {
            get; set;
        }
        public int PortfolioId
        {
            get; set;
        }
        public int AssetTypeId
        {
            get; set;
        }
        public int AssetId
        {
            get; set;
        }
        public DateTime? ScheduledDate
        {
            get; set;
        }
        public DateTime ExecutedDate
        {
            get; set;
        }
        public decimal SipAmount
        {
            get; set;
        }
        public decimal NAVAtExecution
        {
            get; set;
        }
        public decimal UnitsAllocated
        {
            get; set;
        }
        public string? OrderReference
        {
            get; set;
        }
        public int ExecutionStatus
        {
            get; set;
        }
        public string? FailureReason
        {
            get; set;
        }
        public DateTime CreatedDate
        {
            get; set;
        }
        public DateTime? ModifiedDate
        {
            get; set;
        }
    }
}
