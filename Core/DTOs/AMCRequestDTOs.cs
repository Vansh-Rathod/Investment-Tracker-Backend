using System;

namespace Core.DTOs
{
    public class CreateAMCRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string LogoUrl { get; set; }
        public string Description { get; set; }
    }

    public class UpdateAMCRequest : CreateAMCRequest
    {
        public int AmcId { get; set; }
        public bool IsActive { get; set; }
    }
}
