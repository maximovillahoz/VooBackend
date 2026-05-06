using VooApi.Database;
using VooApi.Data;
using VooApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errores = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
            {
                mensaje = "Datos inválidos",
                errores = errores
            });
        };
    });

// SignalR para notificaciones en tiempo real
builder.Services.AddSignalR();

builder.Services.AddScoped<RetoCompletadoRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("VooCors", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true) // 🔥 permite cualquier localhost
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Base de datos
builder.Services.AddSingleton<MongoDbContext>();

// Repositories
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<SalaRepository>();
builder.Services.AddScoped<RetoRepository>();
builder.Services.AddScoped<PremioRepository>();
builder.Services.AddScoped<PoderRepository>();
builder.Services.AddScoped<SolicitudRepository>();
builder.Services.AddScoped<ChatRepository>();
builder.Services.AddScoped<MensajeRepository>();

// Services
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<SalaService>();
builder.Services.AddScoped<RetoService>();
builder.Services.AddScoped<PremioService>();
builder.Services.AddScoped<PoderService>();
builder.Services.AddScoped<SolicitudService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<MensajeService>();
builder.Services.AddScoped<RegistroService>();
builder.Services.AddScoped<RetoFlashService>();
builder.Services.AddScoped<RetoReyService>();
builder.Services.AddScoped<VerdadRetoService>();
builder.Services.AddScoped<PresenciaService>();
builder.Services.AddScoped<RetoGeneratorService>();
builder.Services.AddHostedService<RetoGeneratorBackgroundService>();
builder.Services.AddHostedService<RetoBackgroundService>();

// BackgroundService
builder.Services.AddHostedService<PresenciaBackgroundService>();

var app = builder.Build();

app.UseCors("VooCors");
app.UseHttpsRedirection();
app.MapHub<VooApi.Hubs.SalaHub>("/hubs/sala");
app.MapControllers();
app.Run();