using Core.CommonModels;
using Core.DTOs;
using Core.Entities;
using Core.ViewModels;
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
        private readonly ISipExecutionRepository _sipExecutionRepository;
        private readonly ILoggingService _loggingService;

        public SIPController(ISIPRepository sipRepository, ISipExecutionRepository sipExecutionRepository, ILoggingService loggingService)
        {
            _sipRepository = sipRepository;
            _sipExecutionRepository = sipExecutionRepository;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get all SIPs for the authenticated user with optional filters
        /// </summary>
        [HttpGet("GetSIPs")]
        public async Task<APIResponse<List<SIPViewModel>>> GetSIPs(
            int sipId = 0,
            int sipStatus = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<SIPViewModel>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var sips = await _sipRepository.GetUserSIPsAsync(userId, sipId, sipStatus);

                if (!sips.Success)
                {
                    return APIResponse<List<SIPViewModel>>.FailureResponse(
                        sips.Errors,
                        sips.Message ?? "No SIPs found"
                   );
                }

                if (sips.Data == null || !sips.Data.Any())
                {
                    return APIResponse<List<SIPViewModel>>.FailureResponse(
                    new List<string> { "No SIPs found" },
                    "No SIPs found"
                );
                }

                return APIResponse<List<SIPViewModel>>.SuccessResponse(
                    sips.Data,
                    "SIPs fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching SIPs",
                    Core.Enums.Enum.LogLevel.Critical,
                    "SIPController.GetSIPs",
                    ex.Message,
                    null
                );

                return APIResponse<List<SIPViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching SIPs"
                );
            }
        }

        /// <summary>
        /// Get SIP executions for the authenticated user (optional filters: sipId, executionStatus, fromDate, toDate)
        /// </summary>
        [HttpGet("executions")]
        public async Task<APIResponse<List<SipExecutionViewModel>>> GetSipExecutions(
            int sipId = 0,
            int executionStatus = 0,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<List<SipExecutionViewModel>>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var result = await _sipExecutionRepository.GetUserSipExecutionsAsync(userId, sipId, executionStatus, fromDate, toDate);

                if (!result.Success)
                {
                    return APIResponse<List<SipExecutionViewModel>>.FailureResponse(
                        result.Errors,
                        result.Message ?? "No SIP executions found"
                    );
                }

                return APIResponse<List<SipExecutionViewModel>>.SuccessResponse(
                    result.Data ?? new List<SipExecutionViewModel>(),
                    "SIP executions fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching SIP executions",
                    Core.Enums.Enum.LogLevel.Critical,
                    "SIPController.GetSipExecutions",
                    ex.Message,
                    null
                );

                return APIResponse<List<SipExecutionViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching SIP executions"
                );
            }
        }

        /// <summary>
        /// Get a specific SIP by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<APIResponse<SIPViewModel>> GetSIPById([FromRoute] int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return APIResponse<SIPViewModel>.FailureResponse(
                        new List<string> { "Invalid token" },
                        "Cannot find valid User Id in token"
                    );
                }

                var result = await _sipRepository.GetUserSIPsAsync(userId, id);

                if (!result.Success)
                {
                    return APIResponse<SIPViewModel>.FailureResponse(
                    result.Errors,
                    result.Message ?? $"No SIP found by id : {id}"
                );
                }

                if (result.Data == null || !result.Data.Any())
                {
                    return APIResponse<SIPViewModel>.FailureResponse(
                    new List<string> { $"No SIP found by id: {id}" },
                    $"No SIP found by id: {id}"
                );
                }

                return APIResponse<SIPViewModel>.SuccessResponse(
                    result.Data.FirstOrDefault(),
                    "SIP fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching SIP",
                    Core.Enums.Enum.LogLevel.Critical,
                    "SIPController.GetSIPById",
                    ex.Message,
                    new Dictionary<string, object> { { "SIPId", id } }
                );

                return APIResponse<SIPViewModel>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching the SIP"
                );
            }
        }

        /// <summary>
        /// Create a new SIP
        /// </summary>
        [HttpPost("CreateSIP")]
        [Authorize(Roles = "Admin")]
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

                SIP sip = new SIP
                {
                    UserId = userId,
                    AssetTypeId = request.AssetTypeId,
                    AssetId = request.AssetId,
                    SipAmount = request.SipAmount,
                    Frequency = request.Frequency,
                    SipDate = request.SipDate,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Status = 1 // Active
                };

                var result = await _sipRepository.InsertUpdateDeleteSIP(sip, Core.Enums.Enum.OperationType.INSERT);

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { "Failed to create SIP" },
                    "Failed to create SIP"
                );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "SIP created successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error creating SIP",
                    Core.Enums.Enum.LogLevel.Critical,
                    "SIPController.CreateSIP",
                    ex.Message,
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
        [Authorize(Roles = "Admin")]
        public async Task<APIResponse<int>> UpdateSIP([FromRoute] int id, [FromBody] UpdateSIPRequest request)
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

                request.SipId = id;

                // Check ownership
                var existingSIPResult = await _sipRepository.GetUserSIPsAsync(userId, id);
                if (!existingSIPResult.Success || existingSIPResult.Data == null || !existingSIPResult.Data.Any())
                {
                     return APIResponse<int>.FailureResponse(
                        new List<string> { "SIP not found" },
                        $"SIP with ID {id} not found"
                    );
                }

                SIP sip = new SIP
                {
                    SipId = id,
                    UserId = userId,
                    AssetTypeId = request.AssetTypeId,
                    AssetId = request.AssetId,
                    SipAmount = request.SipAmount,
                    Frequency = request.Frequency,
                    SipDate = request.SipDate,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Status = request.Status
                };

                var result = await _sipRepository.InsertUpdateDeleteSIP(sip, Core.Enums.Enum.OperationType.UPDATE);

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "Failed to update SIP" },
                        "Failed to update SIP"
                    );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "SIP updated successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error updating SIP",
                    Core.Enums.Enum.LogLevel.Critical,
                    "SIPController.UpdateSIP",
                    ex.Message,
                    new Dictionary<string, object> { { "SIPId", id }, { "Request", request } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while updating the SIP"
                );
            }
        }

        /// <summary>
        /// Pause a SIP
        /// </summary>
        [HttpPut("{id}/pause")]
        public async Task<APIResponse<int>> PauseSIP(int id)
        {
            return await UpdateSIPStatus(id, 2, "pause");
        }

        /// <summary>
        /// Resume a SIP
        /// </summary>
        [HttpPut("{id}/resume")]
        public async Task<APIResponse<int>> ResumeSIP(int id)
        {
            return await UpdateSIPStatus(id, 1, "resume");
        }

        /// <summary>
        /// Cancel a SIP
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<APIResponse<int>> CancelSIP(int id)
        {
            return await UpdateSIPStatus(id, 3, "cancel");
        }

        private async Task<APIResponse<int>> UpdateSIPStatus(int sipId, int status, string action)
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

                // Get existing SIP to preserve other fields
                var existingSIPResult = await _sipRepository.GetUserSIPsAsync(userId, sipId);
                if (!existingSIPResult.Success || existingSIPResult.Data == null || !existingSIPResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                       new List<string> { "SIP not found" },
                       $"SIP with ID {sipId} not found"
                   );
                }

                var existingSIP = existingSIPResult.Data.FirstOrDefault();
                
                SIP sip = new SIP
                {
                    SipId = existingSIP.SipId,
                    UserId = existingSIP.UserId,
                    AssetTypeId = existingSIP.AssetTypeId,
                    AssetId = existingSIP.AssetId,
                    SipAmount = existingSIP.SipAmount,
                    Frequency = existingSIP.Frequency,
                    SipDate = existingSIP.SipDate,
                    StartDate = existingSIP.StartDate,
                    EndDate = existingSIP.EndDate,
                    Status = status
                };

                var result = await _sipRepository.InsertUpdateDeleteSIP(sip, Core.Enums.Enum.OperationType.UPDATE);

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { $"Failed to {action} SIP" },
                        $"Failed to {action} SIP"
                    );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    $"SIP {action}d successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    $"Error {action}ing SIP",
                    Core.Enums.Enum.LogLevel.Critical,
                    $"SIPController.{char.ToUpper(action[0]) + action.Substring(1)}SIP",
                    ex.Message,
                    new Dictionary<string, object> { { "SIPId", sipId }, { "Status", status } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    $"An error occurred while {action}ing the SIP"
                );
            }
        }

        /// <summary>
        /// Delete a SIP
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<APIResponse<int>> DeleteSIP(int id)
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

                // Check ownership
                var existingSIPResult = await _sipRepository.GetUserSIPsAsync(userId, id);
                if (!existingSIPResult.Success || existingSIPResult.Data == null || !existingSIPResult.Data.Any())
                {
                     return APIResponse<int>.FailureResponse(
                        new List<string> { "SIP not found" },
                        $"SIP with ID {id} not found"
                    );
                }

                SIP sip = new SIP
                {
                    SipId = id
                };

                var result = await _sipRepository.InsertUpdateDeleteSIP(sip, Core.Enums.Enum.OperationType.DELETE);

                 if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "Failed to delete SIP" },
                        "Failed to delete SIP"
                    );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "SIP deleted successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error deleting SIP",
                    Core.Enums.Enum.LogLevel.Critical,
                    "SIPController.DeleteSIP",
                    ex.Message,
                    new Dictionary<string, object> { { "SIPId", id } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while deleting the SIP"
                );
            }
        }
    }
}
