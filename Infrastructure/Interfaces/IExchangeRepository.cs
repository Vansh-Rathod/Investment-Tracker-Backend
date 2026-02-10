using Core.CommonModels;
using Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IExchangeRepository
    {
        Task<DbResponse<List<ExchangeViewModel>>> GetExchanges(int exchangeId = 0, string exchangeName = null);
    }
}
