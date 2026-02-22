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
    [Authorize(Roles = "Admin")]
    [Authorize]
    public class ExchangeController : ControllerBase
    {
        private readonly IExchangeRepository _exchangeRepository;
        private readonly ILoggingService _loggingService;

        public ExchangeController(IExchangeRepository exchangeRepository, ILoggingService loggingService)
        {
            _exchangeRepository = exchangeRepository;
            _loggingService = loggingService;
        }

        [HttpGet]
        public async Task<APIResponse<List<ExchangeViewModel>>> GetExchanges([FromQuery] int exchangeId = 0, [FromQuery] string exchangeName = null)
        {
            try
            {
                var result = await _exchangeRepository.GetExchanges(exchangeId, exchangeName);
                if (result.Success)
                {
                    return APIResponse<List<ExchangeViewModel>>.SuccessResponse(result.Data, result.Message);
                }
                return APIResponse<List<ExchangeViewModel>>.FailureResponse(result.Errors, result.Message);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Exception occurred while fetching Exchanges", Core.Enums.Enum.LogLevel.Error, "ExchangeController.GetExchanges", ex.Message, null);
                return APIResponse<List<ExchangeViewModel>>.FailureResponse(new List<string> { "Internal Server Error" }, "An error occurred while fetching Exchanges.");
            }
        }
    }
}
