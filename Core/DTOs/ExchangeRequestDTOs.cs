using System;

namespace Core.DTOs
{
    public class CreateExchangeRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
    }

    public class UpdateExchangeRequest : CreateExchangeRequest
    {
        public int ExchangeId { get; set; }
        public bool IsActive { get; set; }
    }
}
