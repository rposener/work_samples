
using TodoApi.Data;

namespace TodoApi
{
    /// <summary>
    /// Class to Verify the Azure Tables Exist at startup
    /// </summary>
    public class AppInitialize : BackgroundService
    {
        private readonly ILogger<AppInitialize> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public AppInitialize(ILogger<AppInitialize> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Checking to ensure azure table exists.");
            using(var scope = _scopeFactory.CreateScope())
            {
                IRepositoryInitializer repositoryInitializer = 
                    scope.ServiceProvider.GetRequiredService<IRepositoryInitializer>();
                await repositoryInitializer.EnsureCreatedAsync();
            }
            _logger.LogInformation("Azure tables all exist.");
        }
    }
}
