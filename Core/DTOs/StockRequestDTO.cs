using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class CreateStockRequest
    {
        public string Name
        {
            get; set;
        }
        public string Symbol
        {
            get; set;
        }
        public int ExchangeId
        {
            get; set;
        }

        //public int SectorId { get; set; }
        //public decimal CurrentPrice { get; set; }
        //public decimal MarketCap { get; set; }
        //public decimal PeRatio { get; set; }
        //public decimal DividendYield { get; set; }
        //public string Description { get; set; }
        public string? ISIN
        {
            get; set;
        }
        public bool IsActive
        {
            get; set;
        }
        public bool IsETF
        {
            get; set;
        }
    }

    public class UpdateStockRequest : CreateStockRequest
    {
        public int StockId
        {
            get; set;
        }
    }
}
