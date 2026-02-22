using Azure.Core;
using Core.CommonModels;
using Core.DTOs;
using Core.Entities;
using Core.ViewModels;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Text;

namespace EquityTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [HttpGet("GetAssetManagementCompanies")]
        public async Task<APIResponse<List<AMCViewModel>>> GetAssetManagementCompanies([FromQuery] string amcName = null)
        {
            try
            {
                var result = await _amcRepository.GetAssetManagementCompanies(0, amcName);

                if (!result.Success)
                {
                    return APIResponse<List<AMCViewModel>>.FailureResponse(
                    result.Errors,
                    result.Message ?? "No Asset Management Companies found"
                );
                }

                if (result.Data == null || !result.Data.Any())
                {
                    return APIResponse<List<AMCViewModel>>.FailureResponse(
                    new List<string> { "No Asset Management Companies found" },
                    "No Asset Management Companies found"
                );
                }

                return APIResponse<List<AMCViewModel>>.SuccessResponse(
                    result.Data,
                    "Asset Management Companies fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching Asset Management Companies",
                    Core.Enums.Enum.LogLevel.Critical,
                    "AMCController.GetAssetManagementCompanies",
                    ex.Message,
                    null
                );

                return APIResponse<List<AMCViewModel>>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching Asset Management Companies"
                );
            }
        }

        /// <summary>
        /// Get AMC by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<APIResponse<AMCViewModel>> GetAssetManagementCompanyById([FromRoute] int id)
        {
            try
            {
                var result = await _amcRepository.GetAssetManagementCompanies(id);

                if (!result.Success)
                {
                    return APIResponse<AMCViewModel>.FailureResponse(
                    result.Errors,
                    result.Message ?? $"No Asset Management Company found by id : {id}"
                );
                }

                if (result.Data == null || !result.Data.Any())
                {
                    return APIResponse<AMCViewModel>.FailureResponse(
                    new List<string> { $"No Asset Management Company found by id: {id}" },
                    $"No Asset Management Company found by id: {id}"
                );
                }

                var amc = result.Data.FirstOrDefault();

                return APIResponse<AMCViewModel>.SuccessResponse(
                    amc,
                    "Asset Management Company fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error fetching Asset Management Company",
                    Core.Enums.Enum.LogLevel.Critical,
                    "AMCController.GetAssetManagementCompanyById",
                    ex.Message,
                    new Dictionary<string, object> { { "AMCId", id } }
                );

                return APIResponse<AMCViewModel>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while fetching the Asset Management Company"
                );
            }
        }

        /// <summary>
        /// Create a new AMC
        /// </summary>
        [HttpPost("CreateAssetManagementCompany")]
        [Authorize(Roles = "Admin")]
        public async Task<APIResponse<int>> CreateAssetManagementCompany([FromBody] CreateAMCRequest request)
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

                AMC amc = new AMC
                {
                    Name = request.Name
                };

                var result = await _amcRepository.InsertUpdateDeleteAMC(amc, Core.Enums.Enum.OperationType.INSERT);

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { "Failed to create Asset Management Company" },
                    "Failed to create Asset Management Company"
                );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Asset Management Company created successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error creating Asset Management Company",
                    Core.Enums.Enum.LogLevel.Critical,
                    "AMCController.CreateAssetManagementCompany",
                    ex.Message,
                    new Dictionary<string, object> { { "Request", request } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while creating the Asset Management Company"
                );
            }
        }

        /// <summary>
        /// Update an existing AMC
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<APIResponse<int>> UpdateAssetManagementCompany([FromRoute] int id, [FromBody] UpdateAMCRequest request)
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

                request.AmcId = id;

                var existingAMCResult = await _amcRepository.GetAssetManagementCompanies(id);

                if (!existingAMCResult.Success)
                {
                    return APIResponse<int>.FailureResponse(
                    existingAMCResult.Errors,
                    existingAMCResult.Message ?? $"No Asset Management Company found by id : {id}"
                    );
                }

                if (existingAMCResult.Data == null || !existingAMCResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { $"No Asset Management Company found by id: {id}" },
                    $"No Asset Management Company found by id: {id}"
                    );
                }

                AMC amc = new AMC
                {
                    AmcId = id,
                    Name = request.Name
                };

                var result = await _amcRepository.InsertUpdateDeleteAMC(amc, Core.Enums.Enum.OperationType.UPDATE);

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { "Failed to update Asset Management Company" },
                    "Failed to update Asset Management Company"
                );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Asset Management Company updated successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error updating Asset Management Company",
                    Core.Enums.Enum.LogLevel.Critical,
                    "AMCController.UpdateAssetManagementCompany",
                    ex.Message,
                    new Dictionary<string, object> { { "AMCId", id }, { "Request", request } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while updating the Asset Management Company"
                );
            }
        }

        /// <summary>
        /// Delete an AMC
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<APIResponse<int>> DeleteAssetManagementCompany(int id)
        {
            try
            {
                var existingAMCResult = await _amcRepository.GetAssetManagementCompanies(id);

                if (!existingAMCResult.Success)
                {
                    return APIResponse<int>.FailureResponse(
                    existingAMCResult.Errors,
                    existingAMCResult.Message ?? $"No Asset Management Company found by id : {id}"
                    );
                }

                if (existingAMCResult.Data == null || !existingAMCResult.Data.Any())
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { $"No Asset Management Company found by id: {id}" },
                    $"No Asset Management Company found by id: {id}"
                    );
                }

                AMC amc = new AMC
                {
                    AmcId = id
                };

                var result = await _amcRepository.InsertUpdateDeleteAMC(amc, Core.Enums.Enum.OperationType.DELETE);

                if (!result.Success || result.Data == 0)
                {
                    return APIResponse<int>.FailureResponse(
                    new List<string> { "Failed to delete Asset Management Company" },
                    "Failed to delete Asset Management Company"
                );
                }

                return APIResponse<int>.SuccessResponse(
                    result.Data,
                    "Asset Management Company deleted successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync(
                    "Error deleting Asset Management Company",
                    Core.Enums.Enum.LogLevel.Critical,
                    "AMCController.DeleteAssetManagementCompany",
                    ex.Message,
                    new Dictionary<string, object> { { "AMCId", id } }
                );

                return APIResponse<int>.FailureResponse(
                    new List<string> { "Internal Server Error" },
                    "An error occurred while deleting the Asset Management Company"
                );
            }
        }
    }
}
