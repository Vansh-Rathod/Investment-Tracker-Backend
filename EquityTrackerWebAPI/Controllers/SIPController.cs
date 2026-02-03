using Core.CommonModels;
using Core.DTOs;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquityTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SIPController : ControllerBase
    {
        private readonly ISIPRepository _sipRepository;
        private readonly ILoggingService _loggingService;

        public SIPController(ISIPRepository sipRepository, ILoggingService loggingService)
        {
            _sipRepository = sipRepository;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get all SIPs for the authenticated user with optional filters
        /// </summary>
        [HttpGet]
        public async Task<APIResponse<List<SIPDTO>>> GetSIPs(
            int sipId = 0,
            int portfolioId = 0,
            int sipStatus = 0,
            int portfolioType = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<SIPDTO>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var sips = await _sipRepository.GetUserSIPsAsync(userId, sipId, portfolioId, sipStatus, portfolioType);

                return APIResponse<List<SIPDTO>>.SuccessResponse(
                    sips.ToList(),
                    "SIPs fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching SIPs",
                    Core.Enums.Enum.LogLevel.Error,
                    "SIPController.GetSIPs",
                    ex,
                    null
                );

                return APIResponse<List<SIPDTO>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching SIPs"
                );
            }
        }

        /// <summary>
        /// Get a specific SIP by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<APIResponse<SIPDTO>> GetSIP(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<SIPDTO>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var sip = await _sipRepository.GetSIPByIdAsync(id, userId);

                if (sip == null)
                {
                    return APIResponse<SIPDTO>.FailureResponse(
                        new List<string> { "SIP not found" },
                        $"SIP with ID {id} not found"
                    );
                }

                return APIResponse<SIPDTO>.SuccessResponse(
                    sip,
                    "SIP fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching SIP",
                    Core.Enums.Enum.LogLevel.Error,
                    "SIPController.GetSIP",
                    ex,
                    new Dictionary<string, object> { { "SIPId", id } }
                );

                return APIResponse<SIPDTO>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching the SIP"
                );
            }
        }

        /// <summary>
        /// Create a new SIP
        /// </summary>
        [HttpPost]
        public async Task<APIResponse<int>> CreateSIP([FromBody] CreateSIPRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                if (request.SipAmount <= 0)
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "Validation failed" },
                        "SIP amount must be greater than zero"
                    );
                }

                var sipId = await _sipRepository.CreateSIPAsync(userId, request);

                return APIResponse<int>.SuccessResponse(
                    sipId,
                    "SIP created successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error creating SIP",
                    Core.Enums.Enum.LogLevel.Error,
                    "SIPController.CreateSIP",
                    ex,
                    new Dictionary<string, object> { { "Request", request } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while creating the SIP"
                );
            }
        }

        /// <summary>
        /// Update an existing SIP
        /// </summary>
        [HttpPut("{id}")]
        public async Task<APIResponse<bool>> UpdateSIP(int id, [FromBody] UpdateSIPRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                request.SipId = id;
                var success = await _sipRepository.UpdateSIPAsync(userId, request);

                if (!success)
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Update failed" },
                        "Failed to update SIP"
                    );
                }

                return APIResponse<bool>.SuccessResponse(
                    true,
                    "SIP updated successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error updating SIP",
                    Core.Enums.Enum.LogLevel.Error,
                    "SIPController.UpdateSIP",
                    ex,
                    new Dictionary<string, object> { { "SIPId", id }, { "Request", request } }
                );

                return APIResponse<bool>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while updating the SIP"
                );
            }
        }

        /// <summary>
        /// Pause a SIP
        /// </summary>
        [HttpPut("{id}/pause")]
        public async Task<APIResponse<bool>> PauseSIP(int id)
        {
            return await UpdateSIPStatus(id, 2, "pause");
        }

        /// <summary>
        /// Resume a SIP
        /// </summary>
        [HttpPut("{id}/resume")]
        public async Task<APIResponse<bool>> ResumeSIP(int id)
        {
            return await UpdateSIPStatus(id, 1, "resume");
        }

        /// <summary>
        /// Cancel a SIP
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<APIResponse<bool>> CancelSIP(int id)
        {
            return await UpdateSIPStatus(id, 3, "cancel");
        }

        private async Task<APIResponse<bool>> UpdateSIPStatus(int sipId, int status, string action)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var success = await _sipRepository.UpdateSIPStatusAsync(userId, sipId, status);

                if (!success)
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Update failed" },
                        $"Failed to {action} SIP"
                    );
                }

                return APIResponse<bool>.SuccessResponse(
                    true,
                    $"SIP {action}d successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    $"Error {action}ing SIP",
                    Core.Enums.Enum.LogLevel.Error,
                    $"SIPController.{char.ToUpper(action[0]) + action.Substring(1)}SIP",
                    ex,
                    new Dictionary<string, object> { { "SIPId", sipId }, { "Status", status } }
                );

                return APIResponse<bool>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    $"An error occurred while {action}ing the SIP"
                );
            }
        }

        /// <summary>
        /// Delete a SIP
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<APIResponse<bool>> DeleteSIP(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var success = await _sipRepository.DeleteSIPAsync(id, userId);

                if (!success)
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Delete failed" },
                        "Failed to delete SIP"
                    );
                }

                return APIResponse<bool>.SuccessResponse(
                    true,
                    "SIP deleted successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error deleting SIP",
                    Core.Enums.Enum.LogLevel.Error,
                    "SIPController.DeleteSIP",
                    ex,
                    new Dictionary<string, object> { { "SIPId", id } }
                );

                return APIResponse<bool>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while deleting the SIP"
                );
            }
        }
    }
}
