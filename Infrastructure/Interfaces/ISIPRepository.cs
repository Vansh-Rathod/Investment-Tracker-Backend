using Core.CommonModels;
using Core.ViewModels;
using Core.Entities;
using static Core.Enums.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ISIPRepository
    {
        Task<DbResponse<List<SIPViewModel>>> GetUserSIPsAsync(int userId, int sipId = 0, int sipStatus = 0);
        Task<DbResponse<int>> InsertUpdateDeleteSIP(SIP sip, OperationType operationType);
    }
}
