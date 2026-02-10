using Core.CommonModels;
using Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ICategoryRepository
    {
        Task<DbResponse<List<CategoryViewModel>>> GetCategories(int categoryId = 0, int categoryType = 0, string categoryName = null);
    }
}
