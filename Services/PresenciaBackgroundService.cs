namespace VooApi.Services
{
    public class PresenciaBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PresenciaBackgroundService> _logger;

        // Cada cuánto tiempo se ejecuta la verificación
        private readonly TimeSpan _intervalo = TimeSpan.FromHours(3);

        public PresenciaBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<PresenciaBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de verificación de presencia iniciado");

            // Bucle infinito que se ejecuta mientras la API esté corriendo
            while (!stoppingToken.IsCancellationRequested)
            {
                // Espera 3 horas antes de la primera verificación
                await Task.Delay(_intervalo, stoppingToken);

                try
                {
                    _logger.LogInformation("Verificando presencias...");

                    // Creamos un scope para poder usar los services
                    // (los BackgroundServices no pueden inyectar Scoped services directamente)
                    using var scope = _scopeFactory.CreateScope();
                    var presenciaService = scope.ServiceProvider
                        .GetRequiredService<PresenciaService>();

                    await presenciaService.VerificarPresenciasAsync();

                    _logger.LogInformation("Verificación de presencias completada");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al verificar presencias");
                }
            }
        }
    }
}