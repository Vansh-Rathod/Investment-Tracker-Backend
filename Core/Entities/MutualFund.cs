using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class MutualFund
    {
        public int FundId
        {
            get; set;
        }
        public int AmcId
        {
            get; set;
        }
        public string FundName
        {
            get; set;
        }
        public int Category
        {
            get; set;
        }
        public string ISIN
        {
            get; set;
        }
        public bool IsActive
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
