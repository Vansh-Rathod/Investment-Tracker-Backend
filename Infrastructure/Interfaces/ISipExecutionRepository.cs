using Core.CommonModels;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ISipExecutionRepository
    {
        Task<DbResponse<List<SipExecutionViewModel>>> GetUserSipExecutionsAsync(
            int userId,
            int sipId = 0,
            int executionStatus = 0,
            DateTime? fromDate = null,
            DateTime? toDate = null);
    }
}
