using Core.CommonModels;
using Core.Entities;
using Core.ViewModels;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EquityTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggingService _loggingService;

        public UserController(IUserRepository userRepository, ILoggingService loggingService)
        {
            _userRepository = userRepository;
            _loggingService = loggingService;
        }

        [HttpGet("GetUsers")]
        public async Task<APIResponse<List<UserViewModel>>> GetUsers(int page = 1, int pageSize = 10, string searchText = "", string sortOrder = "DESC", string sortField = "CreatedDate", bool isActive = true, bool isDeleted = false)
        {
            try
            {
                var result = await _userRepository.GetUsers(0, isActive, isDeleted, page, pageSize, searchText, sortOrder, sortField);

                if (!result.Success)
                {
                    return APIResponse<List<UserViewModel>>.FailureResponse(
                    result.Errors,
                    result.Message ?? "No Users found"
                );
                }

                if (result.Data == null || !result.Data.Any())
                {
                    return APIResponse<List<UserViewModel>>.FailureResponse(
                    new List<string> { "No Users found" },
                    "No Users found"
                );
                }

                return APIResponse<List<UserViewModel>>.SuccessResponse(
                    result.Data,
                    "Users fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching Users",
                    Core.Enums.Enum.LogLevel.Critical,
                    "UserController.GetUsers",
                    ex.Message,
                    null
                );

                return APIResponse<List<UserViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching Users"
                );
            }
        }

        /// <summary>
        /// Get AMC by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<APIResponse<UserViewModel>> GetUserById([FromRoute] int id)
        {
            try
            {
                var result = await _userRepository.GetUsers(id, true, false);

                if (!result.Success)
                {
                    return APIResponse<UserViewModel>.FailureResponse(
                    result.Errors,
                    result.Message ?? $"No User found by id : {id}"
                );
                }

                if (result.Data == null || !result.Data.Any())
                {
                    return APIResponse<UserViewModel>.FailureResponse(
                    new List<string> { $"No User found by id: {id}" },
                    $"No User found by id: {id}"
                );
                }

                var user = result.Data.FirstOrDefault();

                return APIResponse<UserViewModel>.SuccessResponse(
                    user,
                    "User fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching User",
                    Core.Enums.Enum.LogLevel.Critical,
                    "UserController.GetUserById",
                    ex.Message,
                    new Dictionary<string, object> { { "UserId", id } }
                );

                return APIResponse<UserViewModel>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching the User"
                );
            }
        }

        [HttpGet("GetUserByEmail")]
        public async Task<APIResponse<UserViewModel>> GetUserByEmail(string email, bool isActive = true, bool isDeleted = false)
        {
            try
            {
                var result = await _userRepository.GetUserByEmail(email, isActive, isDeleted);

                if (!result.Success)
                {
                    return APIResponse<UserViewModel>.FailureResponse(
                    result.Errors,
                    result.Message ?? $"No User found by email : {email}"
                );
                }

                if (result.Data == null || !result.Data.Any())
                {
                    return APIResponse<UserViewModel>.FailureResponse(
                    new List<string> { $"No User found by email: {email}" },
                    $"No User found by email: {email}"
                );
                }

                var user = result.Data.FirstOrDefault();

                return APIResponse<UserViewModel>.SuccessResponse(
                    user,
                    "User fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching User by email",
                    Core.Enums.Enum.LogLevel.Critical,
                    "UserController.GetUserByEmail",
                    ex.Message,
                    new Dictionary<string, object> { { "Email", email } }
                );

                return APIResponse<UserViewModel>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching the User by email"
                );
            }
        }
    }
}
