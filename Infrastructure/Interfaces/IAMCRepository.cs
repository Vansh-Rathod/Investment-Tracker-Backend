using Core.CommonModels;
using Core.DTOs;
using Core.Entities;
using Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Core.Enums.Enum;

namespace Infrastructure.Interfaces
{
    public interface IAMCRepository
    {
        Task<DbResponse<List<AMCViewModel>>> GetAssetManagementCompanies(int amcId = 0, string amcName = null);
        Task<DbResponse<int>> InsertUpdateDeleteAMC(AMC amc, OperationType operationType);
    }
}
