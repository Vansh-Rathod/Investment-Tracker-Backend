using Core.DTOs;
using Core.CommonModels;
using Dapper;
using GenericServices.Interfaces;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.ViewModels;

using Core.Entities;
using static Core.Enums.Enum;

namespace Infrastructure.Repositories
{
    public class SIPRepository : ISIPRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public SIPRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        public async Task<DbResponse<List<SIPViewModel>>> GetUserSIPsAsync(int userId, int sipId = 0, int sipStatus = 0)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                parameters.Add("@SipId", sipId);
                parameters.Add("@SipStatus", sipStatus);

                var data = await connection.QueryAsync<SIPViewModel>(
                    "GetUserSIPs",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                var sips = data.ToList();
                var today = DateTime.Today;

                foreach (var sip in sips)
                {
                    if (sip.SipStatus == 1) // Active
                    {
                        try
                        {
                            int sipDay = sip.SipDate.Day;
                            int daysInCurrentMonth = DateTime.DaysInMonth(today.Year, today.Month);
                            int dayToUse = Math.Min(sipDay, daysInCurrentMonth);

                            DateTime nextDate = new DateTime(today.Year, today.Month, dayToUse);

                            if (nextDate < today)
                            {
                                nextDate = nextDate.AddMonths(1);
                                // Re-clamp for next month if AddMonths didn't handle "Month end stickiness" as desired (AddMonths(1) to Jan 31 gives Feb 28. But if we started with Feb 28 and original was Jan 31? .NET AddMonths preserves end-of-month logic if started there? No, strict adding.)
                                // Actually, simpler: just take next month.
                            }
                            sip.NextSipDate = nextDate;
                        }
                        catch
                        {
                            // Fallback
                            sip.NextSipDate = null;
                        }
                    }
                }

                return DbResponse<List<SIPViewModel>>.SuccessDbResponse(
                    sips,
                    "SIPs fetched successfully"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to fetch SIPs", Core.Enums.Enum.LogLevel.Error, "SIPRepository.GetUserSIPsAsync", ex.Message, new Dictionary<string, object>
                {
                    { "UserId", userId },
                    { "SipId", sipId },
                    { "SipStatus", sipStatus }
                });
                return DbResponse<List<SIPViewModel>>.FailureDbResponse(
                    new List<SIPViewModel>(),
                    new List<string> { "Failed to fetch SIPs." },
                    "Exception occurred while fetching SIPs"
                );
            }
        }

        public async Task<DbResponse<int>> InsertUpdateDeleteSIP(SIP sip, OperationType operationType)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@OperationType", (int)operationType);
                parameters.Add("@SipId", sip.SipId);
                parameters.Add("@UserId", sip.UserId);
                parameters.Add("@AssetTypeId", sip.AssetTypeId);
                parameters.Add("@AssetId", sip.AssetId);
                parameters.Add("@SipAmount", sip.SipAmount);
                parameters.Add("@Frequency", sip.Frequency);
                parameters.Add("@SipDate", sip.SipDate);
                parameters.Add("@StartDate", sip.StartDate);
                parameters.Add("@EndDate", sip.EndDate);
                parameters.Add("@Status", sip.Status);

                var resultId = await connection.ExecuteScalarAsync<int>(
                    "InsertUpdateDeleteSIP",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return DbResponse<int>.SuccessDbResponse(
                    resultId,
                    $"SIP {operationType} operation successful"
                );
            }
            catch (Exception ex)
            {
                await _loggingService.LogAsync("Failed to perform SIP operation", Core.Enums.Enum.LogLevel.Error, "SIPRepository.InsertUpdateDeleteSIP", ex.Message, new Dictionary<string, object>
                {
                    { "OperationType", operationType },
                    { "SipId", sip?.SipId },
                    { "UserId", sip?.UserId }
                });
                return DbResponse<int>.FailureDbResponse(0, new List<string> { "Failed to perform SIP operation." }, "Exception occurred");
            }
        }

    }
}
