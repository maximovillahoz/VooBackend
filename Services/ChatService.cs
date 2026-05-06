using VooApi.Models;
using VooApi.Data;
using Microsoft.AspNetCore.SignalR;
using VooApi.Hubs;

namespace VooApi.Services
{
    public class ChatService
    {
        private readonly ChatRepository _chatRepository;
        private readonly MensajeRepository _mensajeRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly SolicitudRepository _solicitudRepository;
        private readonly IHubContext<SalaHub> _hubContext;

        public ChatService(
            ChatRepository chatRepository,
            MensajeRepository mensajeRepository,
            UsuarioRepository usuarioRepository,
            SolicitudRepository solicitudRepository,
            IHubContext<SalaHub> hubContext)
        {
            _chatRepository = chatRepository;
            _mensajeRepository = mensajeRepository;
            _usuarioRepository = usuarioRepository;
            _solicitudRepository = solicitudRepository;
            _hubContext = hubContext;
        }

        // Obtener todos los chats de un usuario con info del otro participante
        // Flutter usa esto para mostrar la lista de chats

        public async Task<Chat> CrearOObtenerChatAsync(string usuarioAId, string usuarioBId)
        {
            var existente = await _chatRepository.ObtenerPorParticipantesAsync(usuarioAId, usuarioBId);

            if (existente != null)
                return existente;

            var nuevoChat = new Chat
            {
                EmisorId = usuarioAId,
                ReceptorId = usuarioBId,
                Activo = true
            };

            await _chatRepository.InsertarAsync(nuevoChat);

            return nuevoChat;
        }

        public async Task<List<ChatResumen>> ObtenerChatsDeUsuarioAsync(string usuarioId)
        {
            var chats = await _chatRepository.ObtenerPorUsuarioAsync(usuarioId);
            var resultado = new List<ChatResumen>();

            foreach (var chat in chats)
            {
                // Buscamos al otro participante del chat
                var otroId = chat.EmisorId == usuarioId
                    ? chat.ReceptorId
                    : chat.EmisorId;

                var otro = await _usuarioRepository.ObtenerPorIdAsync(otroId);
                if (otro == null) continue;

                // Buscamos el último mensaje del chat
                var mensajes = await _mensajeRepository.ObtenerPorChatAsync(chat.Id!);
                var ultimoMensaje = mensajes.LastOrDefault();

                // Contamos mensajes no leídos
                var noLeidos = mensajes.Count(m => !m.Leido && m.EmisorId != usuarioId);

                resultado.Add(new ChatResumen
                {
                    ChatId = chat.Id!,
                    OtroUsuarioId = otro.Id!,
                    OtroUsuarioNombre = otro.Nombre,
                    OtroUsuarioFoto = otro.Foto,
                    OtroUsuarioEstado = otro.Estado,
                    UltimoMensaje = ultimoMensaje?.Contenido ?? "",
                    UltimoMensajeFecha = ultimoMensaje?.FechaHora,
                    MensajesNoLeidos = noLeidos,
                    Activo = chat.Activo
                });
            }

            // Ordenamos por fecha del último mensaje
            return resultado.OrderByDescending(c => c.UltimoMensajeFecha).ToList();
        }

        // Obtener todos los mensajes de un chat
        // Flutter los muestra ordenados por fecha
        public async Task<List<Mensaje>> ObtenerMensajesAsync(string chatId, string usuarioId)
        {
            // Marcar todos los mensajes no leídos como leídos
            var mensajes = await _mensajeRepository.ObtenerPorChatAsync(chatId);
            foreach (var mensaje in mensajes.Where(m => !m.Leido && m.EmisorId != usuarioId))
            {
                await _mensajeRepository.MarcarLeidoAsync(mensaje.Id!);
            }
            return mensajes;
        }

        // Enviar un mensaje de texto normal
        public async Task<EnviarMensajeResultado> EnviarMensajeAsync(EnviarMensajeDto dto)
        {
            // 1. Comprobar que el chat existe y está activo
            var chat = await _chatRepository.ObtenerPorIdAsync(dto.ChatId);
            if (chat == null)
                return new EnviarMensajeResultado
                {
                    Exito = false,
                    Mensaje = "Chat no encontrado"
                };

            if (!chat.Activo)
                return new EnviarMensajeResultado
                {
                    Exito = false,
                    Mensaje = "Este chat está desactivado"
                };

            // 2. Verificar que el emisor no está bloqueado
            // Un usuario está bloqueado si tiene una solicitud rechazada
            // y no es el receptor original (quien rechazó puede iniciar)
            var solicitudRechazada = await _solicitudRepository
                .ObtenerPorParticipantesYEstadoAsync(
                    dto.EmisorId, dto.ReceptorId, "rechazado");

            if (solicitudRechazada != null && solicitudRechazada.ReceptorId != dto.EmisorId)
                return new EnviarMensajeResultado
                {
                    Exito = false,
                    Mensaje = "No puedes enviar mensajes a este usuario"
                };

            // 3. Crear y guardar el mensaje
            var mensaje = new Mensaje
            {
                ChatId = dto.ChatId,
                Tipo = "texto",
                EmisorId = dto.EmisorId,
                Contenido = dto.Contenido,
                Leido = false
            };
            await _mensajeRepository.InsertarAsync(mensaje);
            await _hubContext.Clients.Group($"usuario-{dto.ReceptorId}")
            .SendAsync("MensajeRecibido", new
            {
                chatId = dto.ChatId,
                emisorId = dto.EmisorId,
                receptorId = dto.ReceptorId,
                contenido = dto.Contenido,
                fechaHora = mensaje.FechaHora
            });

            return new EnviarMensajeResultado
            {
                Exito = true,
                Mensaje = "Mensaje enviado",
                MensajeEnviado = mensaje
            };
        }

        // Desactivar un chat cuando se rechaza una solicitud
        public async Task DesactivarChatAsync(string chatId)
        {
            await _chatRepository.DesactivarAsync(chatId);
        }
    }
}