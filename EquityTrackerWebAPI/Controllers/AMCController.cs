using Core.CommonModels;
using Core.DTOs;
using Core.ViewModels;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquityTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AMCController : ControllerBase
    {
        private readonly IAMCRepository _amcRepository;
        private readonly ILoggingService _loggingService;

        public AMCController(IAMCRepository amcRepository, ILoggingService loggingService)
        {
            _amcRepository = amcRepository;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get all Asset Management Companies
        /// </summary>
        [HttpGet]
        public async Task<APIResponse<List<AMCViewModel>>> GetAll(string amcName = null)
        {
            try
            {
                var result = await _amcRepository.GetAssetManagementCompanies(amcName: amcName);

                return APIResponse<List<AMCViewModel>>.SuccessResponse(
                    result.Data,
                    result.Message
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching AMCs",
                    Core.Enums.Enum.LogLevel.Error,
                    "AMCController.GetAll",
                    ex,
                    null
                );

                return APIResponse<List<AMCViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching AMCs"
                );
            }
        }

        /// <summary>
        /// Get AMC by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<APIResponse<AMCViewModel>> GetById(int id)
        {
            try
            {
                var amc = await _amcRepository.GetByIdAsync(id);

                if (amc == null)
                {
                    return APIResponse<AMCViewModel>.FailureResponse(
                        new List<string> { "AMC not found" },
                        $"AMC with ID {id} not found"
                    );
                }

                return APIResponse<AMCViewModel>.SuccessResponse(
                    amc,
                    "AMC fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching AMC",
                    Core.Enums.Enum.LogLevel.Error,
                    "AMCController.GetById",
                    ex,
                    new Dictionary<string, object> { { "AMCId", id } }
                );

                return APIResponse<AMCViewModel>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching the AMC"
                );
            }
        }

        /// <summary>
        /// Create a new AMC
        /// </summary>
        [HttpPost]
        public async Task<APIResponse<int>> Create([FromBody] CreateAMCRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return APIResponse<int>.FailureResponse(
                        new List<string> { "Validation failed" },
                        "AMC name is required"
                    );
                }

                var amcId = await _amcRepository.CreateAsync(request);

                return APIResponse<int>.SuccessResponse(
                    amcId,
                    "AMC created successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error creating AMC",
                    Core.Enums.Enum.LogLevel.Error,
                    "AMCController.Create",
                    ex,
                    new Dictionary<string, object> { { "Request", request } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while creating the AMC"
                );
            }
        }

        /// <summary>
        /// Update an existing AMC
        /// </summary>
        [HttpPut("{id}")]
        public async Task<APIResponse<bool>> Update(int id, [FromBody] UpdateAMCRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Validation failed" },
                        "AMC name is required"
                    );
                }

                request.AmcId = id;
                var success = await _amcRepository.UpdateAsync(request);

                if (!success)
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Update failed" },
                        "Failed to update AMC"
                    );
                }

                return APIResponse<bool>.SuccessResponse(
                    true,
                    "AMC updated successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error updating AMC",
                    Core.Enums.Enum.LogLevel.Error,
                    "AMCController.Update",
                    ex,
                    new Dictionary<string, object> { { "AMCId", id }, { "Request", request } }
                );

                return APIResponse<bool>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while updating the AMC"
                );
            }
        }

        /// <summary>
        /// Delete an AMC
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<APIResponse<bool>> Delete(int id)
        {
            try
            {
                var success = await _amcRepository.DeleteAsync(id);

                if (!success)
                {
                    return APIResponse<bool>.FailureResponse(
                        new List<string> { "Delete failed" },
                        "Failed to delete AMC"
                    );
                }

                return APIResponse<bool>.SuccessResponse(
                    true,
                    "AMC deleted successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error deleting AMC",
                    Core.Enums.Enum.LogLevel.Error,
                    "AMCController.Delete",
                    ex,
                    new Dictionary<string, object> { { "AMCId", id } }
                );

                return APIResponse<bool>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while deleting the AMC"
                );
            }
        }
    }
}
