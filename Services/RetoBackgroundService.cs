using Microsoft.AspNetCore.SignalR;
using VooApi.Data;
using VooApi.Hubs;

namespace VooApi.Services
{
    public class RetoBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<SalaHub> _hubContext;

        public RetoBackgroundService(
            IServiceScopeFactory scopeFactory,
            IHubContext<SalaHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var retoService = scope.ServiceProvider.GetRequiredService<RetoService>();

                var huboCambios = await retoService.RotarRetosSiHaceFaltaAsync();

                if (huboCambios)
                {
                    await _hubContext.Clients.All.SendAsync(
                        "RetosActualizados",
                        cancellationToken: stoppingToken
                    );
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}