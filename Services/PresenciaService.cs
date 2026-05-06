using VooApi.Models;
using VooApi.Data;

namespace VooApi.Services
{
    public class PresenciaService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly SalaRepository _salaRepository;

        public PresenciaService(
            UsuarioRepository usuarioRepository,
            SalaRepository salaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _salaRepository = salaRepository;
        }

        // Flutter llama a esto cuando el usuario confirma su ubicación
        public async Task<ResultadoPresencia> ConfirmarPresenciaAsync(ConfirmarPresenciaDto dto)
        {
            // 1. Buscar al usuario
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(dto.UsuarioId);
            if (usuario == null)
                return new ResultadoPresencia
                {
                    Exito = false,
                    Mensaje = "Usuario no encontrado"
                };

            // 2. Buscar la sala para obtener sus coordenadas
            var sala = await _salaRepository.ObtenerPorIdAsync(usuario.SalaId!);
            if (sala == null)
                return new ResultadoPresencia
                {
                    Exito = false,
                    Mensaje = "Sala no encontrada"
                };

            // 3. Comprobar precisión del GPS
            if (dto.Accuracy > 50)
                return new ResultadoPresencia
                {
                    Exito = false,
                    Mensaje = "Ubicación poco precisa. Intenta en otro lugar"
                };

            // 4. Calcular distancia entre usuario y sala
            var distanciaKm = CalcularDistanciaKm(
                dto.Latitud, dto.Longitud,
                sala.Latitud, sala.Longitud);

            // 5. Si está a más de 1km → sacar de la sala
            if (distanciaKm > 1.0)
            {
                await _usuarioRepository.ActualizarPresenciaAsync(
                    dto.UsuarioId, false, DateTime.UtcNow);

                return new ResultadoPresencia
                {
                    Exito = false,
                    DentroRadio = false,
                    Mensaje = $"Estás a {distanciaKm:F2} km de la sala. Has salido de la lista de invitados"
                };
            }

            // 6. Si está dentro del radio → actualizar UltimaVerificacion
            await _usuarioRepository.ActualizarPresenciaAsync(
                dto.UsuarioId, true, DateTime.UtcNow);

            return new ResultadoPresencia
            {
                Exito = true,
                DentroRadio = true,
                Mensaje = "Presencia confirmada. Sigues en la fiesta"
            };
        }

        // El BackgroundService llama a esto cada 3 horas
        // Marca como fuera del radio a los que no han confirmado
        public async Task VerificarPresenciasAsync()
        {
            // Tiempo límite: 3 horas + 10 minutos de margen
            var tiempoLimite = DateTime.UtcNow.AddHours(-3).AddMinutes(-10);

            // Obtener todos los usuarios de todas las salas
            var todosLosUsuarios = await _usuarioRepository.ObtenerTodosAsync();

            foreach (var usuario in todosLosUsuarios)
            {
                // Solo comprobamos invitados que estén dentro del radio
                // El host no necesita verificación
                if (usuario.Tipo == "host") continue;
                if (!usuario.DentroRadio) continue;
                if (usuario.SalaId == null) continue;

                // Si nunca ha verificado o lleva más de 3h10min sin verificar
                var ultimaVerificacion = usuario.UltimaVerificacion ?? DateTime.MinValue;
                if (ultimaVerificacion < tiempoLimite)
                {
                    await _usuarioRepository.ActualizarPresenciaAsync(
                        usuario.Id!, false, DateTime.UtcNow);
                }
            }
        }

        // Fórmula de Haversine para calcular distancia entre dos coordenadas GPS
        private double CalcularDistanciaKm(
            double lat1, double lon1,
            double lat2, double lon2)
        {
            const double radioTierra = 6371;
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return radioTierra * c;
        }

        private double ToRad(double grados) => grados * Math.PI / 180;
    }
}