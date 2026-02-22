using System;

namespace Core.ViewModels
{
    public class UserStockHoldingViewModel
    {
        public int StockId { get; set; }
        public string StockName { get; set; }
        public string Symbol { get; set; }
        public string ExchangeName { get; set; }
        public decimal Quantity { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal InvestedAmount { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal DayChange { get; set; }
        public decimal DayChangePercentage { get; set; }
        public decimal TotalReturn { get; set; }
        public decimal TotalReturnPercentage { get; set; }
    }
}
