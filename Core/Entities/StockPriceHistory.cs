using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class StockPriceHistory
    {
        public int StockId
        {
            get; set;
        }
        public DateTime PriceDate
        {
            get; set;
        }
        public decimal ClosePrice
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
