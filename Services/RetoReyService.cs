using VooApi.Models;
using VooApi.Data;

namespace VooApi.Services
{
    public class RetoReyService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly RetoRepository _retoRepository;

        public RetoReyService(
            UsuarioRepository usuarioRepository,
            RetoRepository retoRepository)
        {
            _usuarioRepository = usuarioRepository;
            _retoRepository = retoRepository;
        }

        // Crear y lanzar el reto a toda la sala
        public async Task<ResultadoReto> CrearRetoAsync(RetoReyDto dto)
        {
            // 1. Comprobar que el creador tiene el poder Rey (>=150 pts)
            var creador = await _usuarioRepository.ObtenerPorIdAsync(dto.CreadorId);
            if (creador == null) return Error("Usuario no encontrado");

            if (creador.Puntos < 150)
                return Error($"Necesitas al menos 150 puntos para usar el poder Rey. Tienes {creador.Puntos}");

            // 2. Construir la descripción del reto según el tipo
            var concepto = dto.TipoReto switch
            {
                "robar_trago"    => "Róbale un trago a alguien con estado amarillo",
                "invitar_bailar" => "Invita a bailar a alguien con estado amarillo",
                "seguir_ig"      => $"Todos los que tienen estado verde síganme en IG: @{dto.IgCreador}",
                "foto_invitado"  => await ConstruirRetoFotoAsync(dto.InvitadoSeleccionadoId),
                _ => null
            };

            if (concepto == null)
                return Error("Tipo de reto desconocido");

            // 3. Crear el reto en MongoDB
            var reto = new Reto
            {
                Tipo = "por invitado",
                Concepto = concepto,
                Puntos = 30,
                HoraActivacion = DateTime.UtcNow,
                EstadoReto = "activo"
            };
            await _retoRepository.InsertarAsync(reto);

            return new ResultadoReto
            {
                Exito = true,
                Mensaje = $"Reto lanzado a toda la sala: {concepto}",
                RetoId = reto.Id
            };
        }

        // El creador escanea el QR del que cumplió el reto
        // y se le suman los puntos al cumplidor
        public async Task<ResultadoReto> VerificarRetoAsync(VerificacionReyDto dto)
        {
            // 1. Comprobar que el creador existe y tiene el poder Rey
            var creador = await _usuarioRepository.ObtenerPorIdAsync(dto.CreadorId);
            if (creador == null) return Error("Creador no encontrado");

            if (creador.Puntos < 150)
                return Error("No tienes el poder Rey para verificar este reto");

            // 2. Comprobar que el reto existe y está activo
            var reto = await _retoRepository.ObtenerPorIdAsync(dto.RetoId);
            if (reto == null) return Error("Reto no encontrado");

            if (reto.EstadoReto != "activo")
                return Error("Este reto ya no está activo");

            // 3. Buscar al cumplidor y sumarle los puntos
            var cumplidor = await _usuarioRepository.ObtenerPorIdAsync(dto.CumplidorId);
            if (cumplidor == null) return Error("Usuario cumplidor no encontrado");

            await SumarPuntosAsync(cumplidor, 30);

            // 4. Marcar el reto como completado
            await _retoRepository.ActualizarEstadoAsync(dto.RetoId, "completado");

            return new ResultadoReto
            {
                Exito = true,
                Mensaje = $"¡{cumplidor.Nombre} ha completado el reto! +30 puntos",
                PuntosGanados = 30
            };
        }

        // Helper para construir el texto del reto de foto
        private async Task<string?> ConstruirRetoFotoAsync(string? invitadoId)
        {
            if (invitadoId == null) return null;
            var invitado = await _usuarioRepository.ObtenerPorIdAsync(invitadoId);
            if (invitado == null) return null;
            return $"Hazte una foto con {invitado.Nombre}";
        }

        private async Task SumarPuntosAsync(Usuario usuario, int puntos)
        {
            usuario.Puntos += puntos;
            usuario.NivelId = usuario.Puntos switch
            {
                >= 150 => "rey",
                >= 100 => "cupido",
                >= 50  => "chismoso",
                _      => "ninguno"
            };
            await _usuarioRepository.ActualizarPuntosAsync(
                usuario.Id!, usuario.Puntos, usuario.NivelId);
        }

        private ResultadoReto Error(string mensaje) =>
            new ResultadoReto { Exito = false, Mensaje = mensaje };
    }
}