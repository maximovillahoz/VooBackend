using VooApi.Models;
using VooApi.Data;

namespace VooApi.Services
{
    public class RetoService
    {
        private static readonly TimeSpan DuracionReto = TimeSpan.FromSeconds(30);

        private readonly RetoRepository _repository;
        private readonly RetoCompletadoRepository _completadoRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly RetoFlashService _retoFlashService;

        public RetoService(
            RetoRepository repository,
            RetoCompletadoRepository completadoRepository,
            UsuarioRepository usuarioRepository,
            RetoFlashService retoFlashService)
        {
            _repository = repository;
            _completadoRepository = completadoRepository;
            _usuarioRepository = usuarioRepository;
            _retoFlashService = retoFlashService;
        }

        public async Task<Reto> CrearAsync(Reto reto)
        {
            reto.Duracion ??= DuracionReto;
            reto.HoraActivacion ??= DateTime.UtcNow;
            await _repository.InsertarAsync(reto);
            return reto;
        }

        public async Task<List<Reto>> ObtenerTodosAsync()
        {
            return await _repository.ObtenerTodosAsync();
        }

        public async Task<List<Reto>> ObtenerActivosAsync()
        {
            return await _repository.ObtenerActivosAsync();
        }

        public async Task<Reto?> ObtenerPorIdAsync(string id)
        {
            return await _repository.ObtenerPorIdAsync(id);
        }

        public async Task ActivarRetoAsync(string id)
        {
            await _repository.ActualizarCamposAsync(id, "activo", DateTime.UtcNow, DuracionReto);
        }

        public async Task CompletarRetoAsync(string id)
        {
            await _repository.ActualizarEstadoAsync(id, "completado");
        }

        public async Task<object> ObtenerTimelineAsync(string usuarioId)
        {
            await RotarRetosSiHaceFaltaAsync();

            var retos = await _repository.ObtenerTodosAsync();

            var activo = retos
                .Where(r => r.EstadoReto == "activo" && r.HoraActivacion != null)
                .OrderByDescending(r => r.HoraActivacion)
                .FirstOrDefault();

            var proximo = retos
                .Where(r => r.EstadoReto == "proximo" && r.HoraActivacion != null)
                .OrderBy(r => r.HoraActivacion)
                .FirstOrDefault();

            var anterior = retos
                .Where(r => r.EstadoReto == "anterior" && r.HoraActivacion != null)
                .OrderByDescending(r => r.HoraActivacion)
                .FirstOrDefault();

            return new
            {
                anterior,
                activo,
                proximo,
                ahora = DateTime.UtcNow
            };
        }

        public async Task<object> CompletarRetoActivoAsync(
            string usuarioId,
            string usuarioEscaneadoId)
        {
            if (usuarioId == usuarioEscaneadoId)
            {
                return new
                {
                    exito = false,
                    mensaje = "No puedes completar un reto escaneando tu propio QR."
                };
            }

            await RotarRetosSiHaceFaltaAsync();

            var retos = await _repository.ObtenerTodosAsync();

            var retoActivo = retos
                .Where(r => r.EstadoReto == "activo" && r.Id != null)
                .OrderByDescending(r => r.HoraActivacion)
                .FirstOrDefault();

            if (retoActivo == null || retoActivo.Id == null)
            {
                return new
                {
                    exito = false,
                    mensaje = "Ahora mismo no hay ningún reto activo."
                };
            }

            var yaCompletado = await _completadoRepository.YaCompletadoAsync(
                retoActivo.Id,
                usuarioId
            );

            if (yaCompletado)
            {
                return new
                {
                    exito = false,
                    mensaje = "Ya has completado el reto actual."
                };
            }

            var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);

            if (usuario == null)
            {
                return new
                {
                    exito = false,
                    mensaje = "Usuario no encontrado."
                };
            }

            var resultadoFlash = await _retoFlashService.VerificarRetoAsync(
                new ProgresoReto
                {
                    UsuarioId = usuarioId,
                    SalaId = usuario.SalaId ?? string.Empty,
                    TipoReto = retoActivo.Tipo,
                    EscaneadosIds = new List<string> { usuarioEscaneadoId }
                }
            );

            if (!resultadoFlash.Exito)
            {
                return new
                {
                    exito = false,
                    mensaje = resultadoFlash.Mensaje
                };
            }

            await _completadoRepository.InsertarAsync(new RetoCompletado
            {
                RetoId = retoActivo.Id,
                UsuarioId = usuarioId,
                UsuarioEscaneadoId = usuarioEscaneadoId,
                FechaCompletado = DateTime.UtcNow
            });

            return new
            {
                exito = true,
                mensaje = resultadoFlash.Mensaje,
                puntosGanados = resultadoFlash.PuntosGanados,
                reto = retoActivo
            };
        }

        public async Task<bool> RotarRetosSiHaceFaltaAsync()
        {
            var retos = await _repository.ObtenerTodosAsync();
            var ahora = DateTime.UtcNow;
            var huboCambios = false;

            var activos = retos
                .Where(r => r.Id != null && r.EstadoReto == "activo" && r.HoraActivacion != null)
                .OrderByDescending(r => r.HoraActivacion)
                .ToList();

            var activo = activos.FirstOrDefault();

            foreach (var extra in activos.Skip(1))
            {
                await _repository.ActualizarEstadoAsync(extra.Id!, "anterior");
                huboCambios = true;
            }

            if (activo != null)
            {
                var fin = activo.HoraActivacion!.Value.Add(DuracionReto);

                if (ahora >= fin)
                {
                    await _repository.ActualizarEstadoAsync(activo.Id!, "anterior");
                    activo = null;
                    huboCambios = true;
                }
            }

            if (activo == null)
            {
                var proximoParaActivar = retos
                    .Where(r => r.Id != null && r.EstadoReto == "proximo")
                    .OrderBy(r => r.HoraActivacion ?? DateTime.MaxValue)
                    .FirstOrDefault();

                if (proximoParaActivar != null)
                {
                    await _repository.ActualizarCamposAsync(
                        proximoParaActivar.Id!,
                        "activo",
                        ahora,
                        DuracionReto
                    );

                    huboCambios = true;
                }
                else
                {
                    await CrearRetoActivoAsync(ahora);
                    huboCambios = true;
                }
            }

            var retosActualizados = await _repository.ObtenerTodosAsync();

            var hayProximo = retosActualizados.Any(r =>
                r.EstadoReto == "proximo" && r.Id != null
            );

            if (!hayProximo)
            {
                await CrearRetoProximoAsync(ahora.Add(DuracionReto));
                huboCambios = true;
            }

            return huboCambios;
        }

        private async Task CrearRetoActivoAsync(DateTime horaActivacion)
        {
            var reto = CrearRetoRandom();
            reto.EstadoReto = "activo";
            reto.HoraActivacion = horaActivacion;
            reto.Duracion = DuracionReto;

            await _repository.InsertarAsync(reto);
        }

        private async Task CrearRetoProximoAsync(DateTime horaActivacion)
        {
            var reto = CrearRetoRandom();
            reto.EstadoReto = "proximo";
            reto.HoraActivacion = horaActivacion;
            reto.Duracion = DuracionReto;

            await _repository.InsertarAsync(reto);
        }

        private Reto CrearRetoRandom()
        {
            var retos = new List<Reto>
            {
                new Reto
                {
                    Tipo = "mismo_estado",
                    Concepto = "Escanea a alguien con tu mismo estado",
                    Puntos = 30,
                },
                new Reto
                {
                    Tipo = "cualquier_escaneo",
                    Concepto = "Escanea el QR de un invitado",
                    Puntos = 25,
                },
                new Reto
                {
                    Tipo = "misma_edad_mismo_estado",
                    Concepto = "Escanea a alguien con tu misma edad y mismo estado",
                    Puntos = 35,
                },
                new Reto
                {
                    Tipo = "mas_joven",
                    Concepto = "Escanea a la persona más joven del evento",
                    Puntos = 40,
                },
                new Reto
                {
                    Tipo = "misma_edad_diferente_estado",
                    Concepto = "Escanea a alguien con tu misma edad pero estado diferente",
                    Puntos = 35,
                },
            };

            return retos[Random.Shared.Next(retos.Count)];
        }
    }
}