using Core.CommonModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ISettingsRepository
    {
        Task<DbResponse<Dictionary<string, string>>> GetUserSettings(int userId);
        Task<DbResponse<bool>> SaveUserSetting(int userId, string key, string value);
    }
}
