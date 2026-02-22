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
    public class AssetTypeController : ControllerBase
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly ILoggingService _loggingService;

        public AssetTypeController(IAssetTypeRepository assetTypeRepository, ILoggingService loggingService)
        {
            _assetTypeRepository = assetTypeRepository;
            _loggingService = loggingService;
        }

        [HttpGet]
        public async Task<APIResponse<List<AssetTypeViewModel>>> GetAssetTypes([FromQuery] int assetTypeId = 0, [FromQuery] string assetName = null)
        {
            try
            {
                var result = await _assetTypeRepository.GetAssetTypes(assetTypeId, assetName);
                if (result.Success)
                {
                    return APIResponse<List<AssetTypeViewModel>>.SuccessResponse(result.Data, result.Message);
                }
                return APIResponse<List<AssetTypeViewModel>>.FailureResponse(result.Errors, result.Message);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Exception occurred while fetching Asset Types", Core.Enums.Enum.LogLevel.Error, "AssetTypeController.GetAssetTypes", ex.Message, null);
                return APIResponse<List<AssetTypeViewModel>>.FailureResponse(new List<string> { "Internal Server Error" }, "An error occurred while fetching Asset Types.");
            }
        }
    }
}
