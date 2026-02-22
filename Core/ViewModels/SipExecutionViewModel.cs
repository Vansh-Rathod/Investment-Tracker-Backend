using System;

namespace Core.ViewModels
{
    public class SipExecutionViewModel
    {
        public int SipExecutionId { get; set; }
        public int SipId { get; set; }
        public int AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public int AssetId { get; set; }
        public string AssetName { get; set; }
        public string AMCName { get; set; }
        public string CategoryName { get; set; }
        public decimal SipAmount { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime ExecutedDate { get; set; }
        public decimal NAVAtExecution { get; set; }
        public decimal UnitsAllocated { get; set; }
        public string OrderReference { get; set; }
        public int ExecutionStatus { get; set; }
        public string ExecutionStatusName { get; set; }
        public string FailureReason { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
