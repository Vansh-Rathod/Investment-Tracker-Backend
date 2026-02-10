using Azure;
using Core.CommonModels;
using Core.DTOs;
using Core.Entities;
using Core.ViewModels;
using EquityTrackerWebAPI.Services;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

namespace EquityTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenService _jwtTokenService;
        private readonly ILoggingService _loggingService;

        public AuthController( IUserRepository userRepository, JwtTokenService jwtTokenService, ILoggingService loggingService )
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            //_appBaseUrl = configuration["AppSettings:APP_BASE_URL"];
            _loggingService = loggingService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<APIResponse<object>> Login( [FromBody] LoginRequestDTO model )
        {
            //var response = new APIResponse<object>();
            try
            {
                // Step 1: Validate input
                if(string.IsNullOrEmpty(model.Email.Trim()) || string.IsNullOrEmpty(model.Password.Trim()))
                {
                    return APIResponse<object>.FailureResponse(
                        new List<string> { "Validation Error" },
                        "Email and Password are required"
                    );
                }

                // Step 2: Check if user exists
                var userResult = await _userRepository.GetUserByEmail(model.Email.Trim(), true, false);

                if(!userResult.Success || userResult.Data == null || !userResult.Data.Any())
                {
                    return APIResponse<object>.FailureResponse(
                        new List<string> { "Validation Failed" },
                        "Invalid email or password"
                    );

                }

                var userData = userResult.Data.FirstOrDefault();

                // Step 3: Check password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password.Trim(), userData.PasswordHash);
                if(!isPasswordValid)
                {
                    return APIResponse<object>.FailureResponse(
                        new List<string> { "Validation Failed" },
                        "Invalid email or password"
                    );
                }

                // Step 4: Update Last Login
                User user = new User()
                {
                    Name = userData.Name,
                    Email = userData.Email,
                    PhoneNumber = userData.PhoneNumber,
                    LastLogin = DateTime.UtcNow,
                    IsActive = userData.IsActive,
                };

                var updateResult = await _userRepository.InsertUpdateDeleteUser(user, Core.Enums.Enum.OperationType.UPDATE, userData.Id);
                //if(!updateResult.Success && updateResult.Data <= 0)
                //{
                    
                //}

                // Step 5: Generate token
                var jwtToken = _jwtTokenService.GenerateJwtToken(userData);
                var refreshToken = _jwtTokenService.GenerateRefreshToken();

                var userLastLogin = userData.LastLogin == null ? DateTime.UtcNow : userData.LastLogin;

                var responseData = new
                {
                    Token = jwtToken,
                    RefreshToken = refreshToken,
                    User = new
                    {
                        userData.Id,
                        userData.Name,
                        userData.Email,
                        userData.PhoneNumber,
                        userLastLogin
                    }
                };

                return APIResponse<object>.SuccessResponse(
                    responseData,
                    "Login Successfull"
                 );
            }
            catch(Exception ex)
            {
                _loggingService.LogAsync("Exception occurred while login", Core.Enums.Enum.LogLevel.Critical, "AuthController.Login", ex, new Dictionary<string, object> { { "Email", model.Email }, { "Password", model.Password } });

                return APIResponse<object>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred during login. Please try again later."
                );
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<APIResponse<object>> Register( [FromBody] RegisterRequestDTO model )
        {
            try
            {
                // Step-1: Validations
                if(string.IsNullOrEmpty(model.Email.Trim()) ||
                    string.IsNullOrEmpty(model.Password.Trim()) ||
                    string.IsNullOrEmpty(model.Name.Trim()))
                {
                    return APIResponse<object>.FailureResponse(
                       new List<string> { "Required fields are missing" },
                       "Email, Password & Name are required"
                   );
                }

                // Step 3: Check if user exists
                var existingUserResult = await _userRepository.GetUserByEmail(model.Email.Trim());
                if(existingUserResult.Success && existingUserResult.Data != null)
                {
                    return APIResponse<object>.FailureResponse(
                       new List<string> { "User with email already exists" },
                       "Duplicate email"
                   );
                }

                // Step 4: Hash password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password.Trim());

                // Step 5: Create new user
                var newUser = new User
                {
                    Name = model.Name.Trim(),
                    Email = model.Email.Trim(),
                    Password = model.Password.Trim(),
                    PasswordHash = hashedPassword,
                    PhoneNumber = model?.PhoneNumber.Trim(),
                    LastLogin = null,
                    IsActive = true
                };

                // Step 7: Save user to DB
                var isCreated = await _userRepository.InsertUpdateDeleteUser(newUser, Core.Enums.Enum.OperationType.INSERT, 0);
                if(!isCreated.Success)
                {
                    return APIResponse<object>.FailureResponse(
                       new List<string> { "User could not be saved to the database" },
                       "User Registration failed"
                   );
                }

                var profileInfo = new
                {
                    UserId = isCreated.Data,
                    Name = newUser.Name.Trim(),
                    Email = newUser.Email.Trim(),
                    PhoneNumber = newUser.PhoneNumber.Trim(),
                    IsActive = newUser.IsActive,
                };

                return APIResponse<object>.SuccessResponse(
                    profileInfo,
                    "Registration Successfull"
                 );
            }
            catch(Exception ex)
            {
                _loggingService.LogAsync("Exception occurred while registration", Core.Enums.Enum.LogLevel.Critical, "AuthController.Register", ex, new Dictionary<string, object> { { "Name", model.Name.Trim() }, { "Email", model.Email.Trim() } });

                return APIResponse<object>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred during registration. Please try again later."
                );
            }
        }

    }
}
