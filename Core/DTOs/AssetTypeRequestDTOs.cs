using System;

namespace Core.DTOs
{
    public class CreateAssetTypeRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateAssetTypeRequest : CreateAssetTypeRequest
    {
        public int AssetTypeId { get; set; }
        public bool IsActive { get; set; }
    }
}
