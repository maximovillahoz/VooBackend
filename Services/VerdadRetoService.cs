using VooApi.Models;
using VooApi.Data;

namespace VooApi.Services
{
    public class VerdadRetoService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly SolicitudRepository _solicitudRepository;
        private readonly ChatRepository _chatRepository;
        private readonly MensajeRepository _mensajeRepository;

        private readonly List<string> _verdades = new()
        {
            "¿Qué pensaste al ver mi foto de perfil?",
            "¿Dejarte llevar o sobrepensar todo?",
            "¿Qué haces cuando alguien te gusta?",
            "¿Qué te suele dar más vergüenza en estos planes?",
            "¿Qué te hace quedarte hablando con alguien?",
            "¿Te consideras más tímido/a o extrovertido/a?",
            "¿Qué canción dirías que encaja contigo hoy?"
        };

        private readonly List<string> _retos = new()
        {
            "Te reto a adivinar donde estoy",
            "Te reto a decir un chiste",
            "Te reto a hacer un mini brindis conmigo",
            "Te reto a hacer una pregunta incómoda",
            "Te reto a decirme 2 verdades y una mentira",
            "Te reto a decirme un secreto",
            "Te reto a ponerme un apodo"
        };

        public VerdadRetoService(
            UsuarioRepository usuarioRepository,
            SolicitudRepository solicitudRepository,
            ChatRepository chatRepository,
            MensajeRepository mensajeRepository)
        {
            _usuarioRepository = usuarioRepository;
            _solicitudRepository = solicitudRepository;
            _chatRepository = chatRepository;
            _mensajeRepository = mensajeRepository;
        }

        // Devuelve una verdad y un reto aleatorios
        public VerdadRetoResultado ObtenerVerdadYReto()
        {
            var random = new Random();
            return new VerdadRetoResultado
            {
                Verdad = _verdades[random.Next(_verdades.Count)],
                Reto = _retos[random.Next(_retos.Count)]
            };
        }

        // Cuando el receptor acepta:
        // 1. Marca la solicitud como aceptada
        // 2. Ambos ganan +5 pts
        // 3. Crea el chat automáticamente
        // 4. Manda el verdad o reto como primer mensaje del chat
        public async Task<AceptarSolicitudResultado> AceptarSolicitudAsync(string solicitudId)
        {
            // 1. Buscar la solicitud
            var solicitud = await _solicitudRepository.ObtenerPorIdAsync(solicitudId);
            if (solicitud == null)
                return new AceptarSolicitudResultado
                {
                    Exito = false,
                    Mensaje = "Solicitud no encontrada"
                };

            // 2. Marcar como aceptada
            await _solicitudRepository.ActualizarEstadoAsync(solicitudId, "aceptado", true);

            // 3. Sumar +5 pts al emisor
            var emisor = await _usuarioRepository.ObtenerPorIdAsync(solicitud.EmisorId);
            if (emisor != null) await SumarPuntosMatchAsync(emisor);

            // 4. Sumar +5 pts al receptor
            var receptor = await _usuarioRepository.ObtenerPorIdAsync(solicitud.ReceptorId);
            if (receptor != null) await SumarPuntosMatchAsync(receptor);

            // 5. Comprobar si ya existe un chat entre estos dos usuarios
            var chatExistente = await _chatRepository.ObtenerPorParticipantesAsync(
                solicitud.EmisorId, solicitud.ReceptorId);

            Chat chat;
            if (chatExistente != null)
            {
                // Si ya existe un chat lo reactivamos
                chat = chatExistente;
            }
            else
            {
                // Si no existe lo creamos
                chat = new Chat
                {
                    EmisorId = solicitud.EmisorId,
                    ReceptorId = solicitud.ReceptorId,
                    Activo = true
                };
                await _chatRepository.InsertarAsync(chat);
            }

            // 6. Mandar el verdad o reto como primer mensaje del chat
            // Esto es lo que el receptor ve cuando entra al chat
            var tipoMensaje = solicitud.Tipo == "truth"
                ? "verdad"
                : solicitud.Tipo == "dare"
                    ? "reto"
                    : "mensaje";

            var primerMensaje = new Mensaje
            {
                ChatId = chat.Id!,
                Tipo = tipoMensaje,
                EmisorId = solicitud.EmisorId,
                Contenido = solicitud.Contenido,
                Leido = false
            };
            await _mensajeRepository.InsertarAsync(primerMensaje);

            return new AceptarSolicitudResultado
            {
                Exito = true,
                Mensaje = "¡Match conseguido! Ambos ganan +5 puntos",
                PuntosGanados = 5,
                ChatId = chat.Id!,
                PrimerMensaje = primerMensaje,
                EmisorId = solicitud.EmisorId,
                ReceptorId = solicitud.ReceptorId
            };
        }

        // Cuando el receptor rechaza:
        // Marca la solicitud como rechazada
        // El emisor no puede volver a escribir a menos que el receptor lo inicie
        public async Task<ResultadoReto> RechazarSolicitudAsync(string solicitudId)
        {
            var solicitud = await _solicitudRepository.ObtenerPorIdAsync(solicitudId);
            if (solicitud == null)
                return new ResultadoReto { Exito = false, Mensaje = "Solicitud no encontrada" };

            await _solicitudRepository.ActualizarEstadoAsync(solicitudId, "rechazado", false);

            return new ResultadoReto
            {
                Exito = true,
                Mensaje = "Está en otra misión, intenta con otro",
                EmisorId = solicitud.EmisorId,
                ReceptorId = solicitud.ReceptorId
            };
        }

        private async Task SumarPuntosMatchAsync(Usuario usuario)
        {
            usuario.Puntos += 5;
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
    }
}