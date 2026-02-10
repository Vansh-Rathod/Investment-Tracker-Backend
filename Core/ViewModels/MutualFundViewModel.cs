using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class MutualFundViewModel
    {
        public int FundId
        {
            get; set;
        }
        public string FundName
        {
            get; set;
        }
        public int AmcId
        {
            get; set;
        }
        public string AssetManagementCompanyName
        {
            get; set;
        }
        public int CategoryId
        {
            get; set;
        }
        public string CategoryName
        {
            get; set;
        }
        public int CategoryType
        {
            get; set;
        }
        public string CategoryTypeName
        {
            get; set;
        }
        //public string ISIN
        //{
        //    get; set;
        //}
        //public bool IsActive
        //{
        //    get; set;
        //}
        public DateTime CreatedDate
        {
            get; set;
        }
        public DateTime? ModifiedDate
        {
            get; set;
        }
        public int TotalRecords
        {
            get; set;
        }
    }
}
