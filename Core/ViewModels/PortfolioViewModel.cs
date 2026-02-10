using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class PortfolioViewModel
    {
        public int PortfolioId { get; set; }
        public string PortfolioName { get; set; }
        public int PortfolioType { get; set; }
        public string PortfolioTypeName { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
    }
}
