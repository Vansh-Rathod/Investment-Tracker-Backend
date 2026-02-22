using System.Threading.Tasks;

namespace EquityTrackerWebAPI.Services
{
    public interface IExportService
    {
        /// <summary>
        /// Generates Excel file for the given user and export type.
        /// </summary>
        /// <param name="userId">User ID (from token).</param>
        /// <param name="exportType">"all" | "stocks" | "mutual-funds".</param>
        /// <returns>Excel file bytes and suggested file name.</returns>
        Task<(byte[] Content, string FileName)> ExportToExcelAsync(int userId, string exportType);
    }
}
