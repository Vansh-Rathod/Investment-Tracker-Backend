using Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ISIPRepository
    {
        Task<IEnumerable<SIPDTO>> GetUserSIPsAsync(int userId, int sipId = 0, int portfolioId = 0, int sipStatus = 0, int portfolioType = 0);
        Task<SIPDTO?> GetSIPByIdAsync(int sipId, int userId);
        Task<int> CreateSIPAsync(int userId, CreateSIPRequest request);
        Task<bool> UpdateSIPAsync(int userId, UpdateSIPRequest request);
        Task<bool> UpdateSIPStatusAsync(int userId, int sipId, int status);
        Task<bool> DeleteSIPAsync(int sipId, int userId);
    }
}
