using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class MutualFundNAVHistory
    {
        public int FundId
        {
            get; set;
        }
        public DateTime NavDate
        {
            get; set;
        }
        public decimal NAV
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
