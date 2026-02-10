using Core.CommonModels;
using Core.ViewModels;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EquityTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILoggingService _loggingService;

        public CategoryController(ICategoryRepository categoryRepository, ILoggingService loggingService)
        {
            _categoryRepository = categoryRepository;
            _loggingService = loggingService;
        }

        [HttpGet]
        public async Task<APIResponse<List<CategoryViewModel>>> GetCategories([FromQuery] int categoryId = 0, [FromQuery] int categoryType = 0, [FromQuery] string categoryName = null)
        {
            try
            {
                var result = await _categoryRepository.GetCategories(categoryId, categoryType, categoryName);
                if (result.Success)
                {
                    return APIResponse<List<CategoryViewModel>>.SuccessResponse(result.Data, result.Message);
                }
                return APIResponse<List<CategoryViewModel>>.FailureResponse(result.Errors, result.Message);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Exception occurred while fetching Categories", Core.Enums.Enum.LogLevel.Error, "CategoryController.GetCategories", ex, null);
                return APIResponse<List<CategoryViewModel>>.FailureResponse(new List<string> { "Internal Server Error" }, "An error occurred while fetching Categories.");
            }
        }
    }
}
