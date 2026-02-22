using EquityTrackerWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EquityTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExportController : ControllerBase
    {
        private readonly IExportService _exportService;

        public ExportController(IExportService exportService)
        {
            _exportService = exportService;
        }

        /// <summary>
        /// Export all (stocks + mutual funds holdings and transactions) to Excel.
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> ExportAll()
        {
            return await ExportInternal("all");
        }

        /// <summary>
        /// Export stock holdings and stock transactions to Excel.
        /// </summary>
        [HttpGet("stocks")]
        public async Task<IActionResult> ExportStocks()
        {
            return await ExportInternal("stocks");
        }

        /// <summary>
        /// Export mutual fund holdings and mutual fund transactions to Excel.
        /// </summary>
        [HttpGet("mutual-funds")]
        public async Task<IActionResult> ExportMutualFunds()
        {
            return await ExportInternal("mutual-funds");
        }

        private async Task<IActionResult> ExportInternal(string exportType)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            try
            {
                var (content, fileName) = await _exportService.ExportToExcelAsync(userId, exportType);
                const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(content, contentType, fileName);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while generating the export.");
            }
        }
    }
}
