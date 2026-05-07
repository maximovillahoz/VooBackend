using Microsoft.AspNetCore.SignalR;

namespace VooApi.Hubs
{
    public class SalaHub : Hub
    {
        public async Task JoinSala(string salaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, salaId);
        }

        public async Task LeaveSala(string salaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, salaId);
        }

        public async Task JoinUsuario(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"usuario-{userId}");
        }

        public async Task NotificarSalaCerrada(string salaId)
        {
            await Clients.Group(salaId).SendAsync("SalaCerrada");
        }

        public async Task EnviarSolicitud(
            string solicitudId,
            string fromUserId,
            string fromUserName,
            string targetUserId,
            string type,
            string content)
        {
            await Clients.Group($"usuario-{targetUserId}").SendAsync("SolicitudRecibida", new
            {
                solicitudId,
                fromUserId,
                fromUserName,
                type,
                content
            });
        }
        public async Task EnviarMensajeChat(
            string chatId,
            string fromUserId,
            string targetUserId,
            string content,
            string time
        )
        {
            await Clients
                .Group($"usuario-{targetUserId}")
                .SendAsync("MensajeChatRecibido", new
                {
                    chatId,
                    fromUserId,
                    content,
                    time
                });
        }

        public async Task NotificarUsuarioEntradoSala(string salaId)
        {
            await Clients.Group(salaId).SendAsync("UsuarioEntradoSala");
        }

        public async Task NotificarRetosActualizados(string salaId)
        {
            await Clients.Group(salaId).SendAsync("RetosActualizados");
        }
    }
}