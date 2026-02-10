using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Stock
    {
        public int StockId
        {
            get; set;
        }
        public string StockName
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
