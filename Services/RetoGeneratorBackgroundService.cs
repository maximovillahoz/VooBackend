using Microsoft.Extensions.Hosting;

namespace VooApi.Services
{
    public class RetoGeneratorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public RetoGeneratorBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var generator = scope.ServiceProvider.GetRequiredService<RetoGeneratorService>();
                    await generator.GenerarRetoAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}