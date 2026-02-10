using Core.CommonModels;
using Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IAssetTypeRepository
    {
        Task<DbResponse<List<AssetTypeViewModel>>> GetAssetTypes(int assetTypeId = 0, string assetName = null);
    }
}
