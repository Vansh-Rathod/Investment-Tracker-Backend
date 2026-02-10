using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class AssetTypeViewModel
    {
        public int AssetTypeId
        {
            get; set;
        }
        public string Name
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
