using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Category
    {
        public int CategoryId
        {
            get; set;
        }
        public int CategoryType
        {
            get; set;
        }
        public string CategoryName
        {
            get; set;
        }
        public string CategoryDescription
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
