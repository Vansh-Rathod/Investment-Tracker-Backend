using Core.CommonModels;
using Core.DTOs;
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
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly IUserRepository _userRepository; // Using UserRepository for simplicity or add ISettingsRepository
        // Actually, let's inject ISettingsRepository which we will create
        private readonly ISettingsRepository _settingsRepository;
        private readonly ILoggingService _loggingService;

        public SettingsController(ISettingsRepository settingsRepository, ILoggingService loggingService)
        {
            _settingsRepository = settingsRepository;
            _loggingService = loggingService;
        }

        [HttpGet]
        public async Task<APIResponse<Dictionary<string, string>>> GetSettings()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<Dictionary<string, string>>.FailureResponse(new List<string> { "Invalid Token" }, "User ID not found");
                }

                var result = await _settingsRepository.GetUserSettings(userId);
                
                return APIResponse<Dictionary<string, string>>.SuccessResponse(result.Data, "Settings fetched successfully");
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error fetching settings", Core.Enums.Enum.LogLevel.Error, "SettingsController.GetSettings", ex.Message, null);
                return APIResponse<Dictionary<string, string>>.FailureResponse(new List<string> { "Internal Error" }, "Failed to fetch settings");
            }
        }

        [HttpPost]
        public async Task<APIResponse<bool>> SaveSetting([FromBody] UserSettingDto setting)
        {
             try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                     return APIResponse<bool>.FailureResponse(new List<string> { "Invalid Token" }, "User ID not found");
                }

                var result = await _settingsRepository.SaveUserSetting(userId, setting.Key, setting.Value);

                if (!result.Success)
                     return APIResponse<bool>.FailureResponse(result.Errors, result.Message);

                return APIResponse<bool>.SuccessResponse(true, "Setting saved successfully");
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Error saving setting", Core.Enums.Enum.LogLevel.Error, "SettingsController.SaveSetting", ex.Message, null);
                return APIResponse<bool>.FailureResponse(new List<string> { "Internal Error" }, "Failed to save setting");
            }
        }
    }
}
