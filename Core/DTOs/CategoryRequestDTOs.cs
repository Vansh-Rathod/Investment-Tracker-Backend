using System;

namespace Core.DTOs
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public int CategoryType { get; set; } // 1=Equity, 2=Debt, 3=Hybrid, 4=Commodities
        public string Description { get; set; }
    }

    public class UpdateCategoryRequest : CreateCategoryRequest
    {
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
    }
}
