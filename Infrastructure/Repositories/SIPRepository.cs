using Core.DTOs;
using Dapper;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SIPRepository : ISIPRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SIPRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<SIPDTO>> GetUserSIPsAsync(int userId, int sipId = 0, int portfolioId = 0, int sipStatus = 0, int portfolioType = 0)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@SipId", sipId);
            parameters.Add("@PortfolioId", portfolioId);
            parameters.Add("@SipStatus", sipStatus);
            parameters.Add("@PortfolioType", portfolioType);

            var result = await connection.QueryAsync<SIPDTO>(
                "GetUserSIPs",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<SIPDTO?> GetSIPByIdAsync(int sipId, int userId)
        {
            var sips = await GetUserSIPsAsync(userId, sipId);
            return sips.FirstOrDefault();
        }

        public async Task<int> CreateSIPAsync(int userId, CreateSIPRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@SipId", null);
            parameters.Add("@PortfolioId", request.PortfolioId);
            parameters.Add("@AssetTypeId", request.AssetTypeId);
            parameters.Add("@AssetId", request.AssetId);
            parameters.Add("@SipAmount", request.SipAmount);
            parameters.Add("@Frequency", request.Frequency);
            parameters.Add("@SipDate", request.SipDate);
            parameters.Add("@StartDate", request.StartDate);
            parameters.Add("@EndDate", request.EndDate);
            parameters.Add("@Status", 1); // Active by default
            parameters.Add("@Operation", "INSERT");

            var sipId = await connection.ExecuteScalarAsync<int>(
                "InsertUpdateDeleteSIP",
                parameters,
                commandType: CommandType.StoredProcedure);

            return sipId;
        }

        public async Task<bool> UpdateSIPAsync(int userId, UpdateSIPRequest request)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@SipId", request.SipId);
            parameters.Add("@SipAmount", request.SipAmount);
            parameters.Add("@Frequency", request.Frequency);
            parameters.Add("@SipDate", request.SipDate);
            parameters.Add("@EndDate", request.EndDate);
            parameters.Add("@Operation", "UPDATE");

            var result = await connection.ExecuteAsync(
                "InsertUpdateDeleteSIP",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> UpdateSIPStatusAsync(int userId, int sipId, int status)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@SipId", sipId);
            parameters.Add("@Status", status);
            parameters.Add("@Operation", "UPDATE_STATUS");

            var result = await connection.ExecuteAsync(
                "InsertUpdateDeleteSIP",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> DeleteSIPAsync(int sipId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@SipId", sipId);
            parameters.Add("@Operation", "DELETE");

            var result = await connection.ExecuteAsync(
                "InsertUpdateDeleteSIP",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result > 0;
        }
    }
}
