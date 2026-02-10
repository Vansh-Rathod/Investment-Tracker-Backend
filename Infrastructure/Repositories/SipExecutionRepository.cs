using GenericServices.Interfaces;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories
{
    public class SipExecutionRepository : ISipExecutionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;

        public SipExecutionRepository(IDbConnectionFactory connectionFactory, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
        }

        // No Get method found in Stored Procedures for SipExecution
    }
}
