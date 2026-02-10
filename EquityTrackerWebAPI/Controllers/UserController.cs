using Core.CommonModels;
using Core.Entities;
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
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggingService _loggingService;

        public UserController(IUserRepository userRepository, ILoggingService loggingService)
        {
            _userRepository = userRepository;
            _loggingService = loggingService;
        }

        [HttpGet]
        public async Task<APIResponse<List<UserViewModel>>> GetUsers([FromQuery] int userId = 0, [FromQuery] bool isActive = true, [FromQuery] bool isDeleted = false, [FromQuery] int page = 0, [FromQuery] int pageSize = 10, [FromQuery] string searchText = "", [FromQuery] string sortOrder = "DESC", [FromQuery] string sortField = "CreatedDate")
        {
            try
            {
                var result = await _userRepository.GetUsers(userId, isActive, isDeleted, page, pageSize, searchText, sortOrder, sortField);
                if (result.Success)
                {
                    return APIResponse<List<UserViewModel>>.SuccessResponse(result.Data, result.Message);
                }
                return APIResponse<List<UserViewModel>>.FailureResponse(result.Errors, result.Message);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Exception occurred while fetching Users", Core.Enums.Enum.LogLevel.Error, "UserController.GetUsers", ex, null);
                return APIResponse<List<UserViewModel>>.FailureResponse(new List<string> { "Internal Server Error" }, "An error occurred while fetching Users.");
            }
        }

        [HttpGet("{email}")]
        public async Task<APIResponse<UserViewModel>> GetUserByEmail(string email, [FromQuery] bool isActive = true, [FromQuery] bool isDeleted = false)
        {
            try
            {
                var result = await _userRepository.GetUserByEmail(email, isActive, isDeleted);
                if (result.Success)
                {
                    return APIResponse<UserViewModel>.SuccessResponse(result.Data, result.Message);
                }
                return APIResponse<UserViewModel>.FailureResponse(result.Errors, result.Message);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Exception occurred while fetching User by Email", Core.Enums.Enum.LogLevel.Error, "UserController.GetUserByEmail", ex, new Dictionary<string, object> { { "Email", email } });
                return APIResponse<UserViewModel>.FailureResponse(new List<string> { "Internal Server Error" }, "An error occurred while fetching User.");
            }
        }
    }
}
