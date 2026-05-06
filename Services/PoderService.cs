using VooApi.Models;
using VooApi.Data;

namespace VooApi.Services
{
    public class PoderService
    {
        private readonly PoderRepository _repository;
        private readonly UsuarioRepository _usuarioRepository;

        public PoderService(PoderRepository repository, UsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        // Devuelve todos los poderes disponibles en el sistema
        public async Task<List<Poder>> ObtenerTodosAsync()
        {
            return await _repository.ObtenerTodosAsync();
        }

        // Dado un usuario, devuelve qué poderes tiene disponibles
        // Flutter usa esto para saber qué botones mostrar
        public async Task<List<Poder>> ObtenerPoderesDisponiblesAsync(string usuarioId)
        {
            // Buscamos al usuario para saber cuántos puntos tiene
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);
            if (usuario == null) return new List<Poder>();

            // Buscamos todos los poderes del sistema
            var todosLosPoderes = await _repository.ObtenerTodosAsync();

            // Filtramos: solo devolvemos los poderes que el usuario
            // puede usar según sus puntos actuales
            return todosLosPoderes
                .Where(p => p.PuntosNecesarios <= usuario.Puntos)
                .ToList();
        }

        // Comprueba si un usuario tiene permiso para usar un poder concreto
        // Flutter llama a esto antes de ejecutar cualquier acción de poder
        public async Task<bool> TienePermisoAsync(string usuarioId, string nivelId)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId);
            if (usuario == null) return false;

            // Comprobamos según el nivel requerido
            return nivelId switch
            {
                "chismoso" => usuario.Puntos >= 50,
                "cupido"   => usuario.Puntos >= 100,
                "rey"      => usuario.Puntos >= 200,
                _          => false
            };
        }

        // Devuelve la info completa de un poder por su nivelId
        // Flutter usa esto para mostrar la descripción del poder en pantalla
        public async Task<Poder?> ObtenerPorNivelAsync(string nivelId)
        {
            var todosLosPoderes = await _repository.ObtenerTodosAsync();
            return todosLosPoderes.FirstOrDefault(p => p.NivelId == nivelId);
        }

        // Crear un poder nuevo en la base de datos
        // Esto solo lo usaréis vosotros para inicializar los 3 poderes en MongoDB
        public async Task<Poder> CrearAsync(Poder poder)
        {
            await _repository.InsertarAsync(poder);
            return poder;
        }
    }
}