using Core.CommonModels;
using Core.DTOs;
using Core.Entities;
using Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Core.Enums.Enum;

namespace Infrastructure.Interfaces
{
    public interface ISIPRepository
    {
        Task<DbResponse<List<SIPViewModel>>> GetUserSIPsAsync(int userId, int sipId = 0, int portfolioId = 0, int sipStatus = 0, int portfolioType = 0);
        Task<DbResponse<int>> InsertUpdateDeleteSIP(SIP sip, OperationType operationType);
    }
}
