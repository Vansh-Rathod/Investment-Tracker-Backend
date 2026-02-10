using System;

namespace Core.ViewModels
{
    public class AllocationDataViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public decimal Percentage { get; set; }
    }
}
