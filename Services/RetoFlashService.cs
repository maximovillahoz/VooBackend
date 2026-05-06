using VooApi.Models;
using VooApi.Data;

namespace VooApi.Services
{
    public class RetoFlashService
    {
        private readonly UsuarioRepository _usuarioRepository;

        public RetoFlashService(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<ResultadoReto> VerificarRetoAsync(ProgresoReto progreso)
        {
            if (progreso.EscaneadosIds == null || progreso.EscaneadosIds.Count == 0)
                return Error("Necesitas escanear a una persona");

            return progreso.TipoReto switch
            {
                "misma_edad_mujer"            => await VerificarMismaEdadMujerAsync(progreso),
                "nombre_a"                    => await VerificarNombreAAsync(progreso),
                "verde_misma_edad"            => await VerificarVerdeMismaEdadAsync(progreso),
                "amarillo_2h_1m"              => await VerificarAmarilloAsync(progreso),
                "match_0pts"                  => await VerificarMatch0PtsAsync(progreso),
                "misma_edad_mismo_estado"     => await VerificarMismaEdadMismoEstadoAsync(progreso),
                "mismo_estado"                => await VerificarMismoEstadoAsync(progreso),
                "mas_joven"                   => await VerificarMasJovenAsync(progreso),
                "misma_edad_diferente_estado" => await VerificarMismaEdadDiferenteEstadoAsync(progreso),
                "cualquier_escaneo"           => await VerificarCualquierEscaneoAsync(progreso),
                _ => Error("Tipo de reto desconocido")
            };
        }

        private async Task<(Usuario? usuario, Usuario? escaneado, ResultadoReto? error)> ObtenerUsuariosAsync(ProgresoReto progreso)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(progreso.UsuarioId);
            if (usuario == null) return (null, null, Error("Usuario no encontrado"));

            var escaneado = await _usuarioRepository.ObtenerPorIdAsync(progreso.EscaneadosIds[0]);
            if (escaneado == null) return (usuario, null, Error("Usuario escaneado no encontrado"));

            if (escaneado.Id == usuario.Id)
                return (usuario, escaneado, Error("No puedes completar un reto escaneándote a ti mismo"));

            if (!string.IsNullOrWhiteSpace(progreso.SalaId) && escaneado.SalaId != progreso.SalaId)
                return (usuario, escaneado, Error("El usuario escaneado no está en tu sala"));

            return (usuario, escaneado, null);
        }

        private async Task<ResultadoReto> VerificarMismaEdadMujerAsync(ProgresoReto progreso)
        {
            var data = await ObtenerUsuariosAsync(progreso);
            if (data.error != null) return data.error;

            var usuario = data.usuario!;
            var escaneado = data.escaneado!;

            if (escaneado.Sexo)
                return Error("La persona escaneada no es mujer");

            if (CalcularEdad(usuario.FechaNacimiento) != CalcularEdad(escaneado.FechaNacimiento))
                return Error("La persona escaneada no tiene tu misma edad");

            return await CompletarAsync(usuario);
        }

        private async Task<ResultadoReto> VerificarNombreAAsync(ProgresoReto progreso)
        {
            var data = await ObtenerUsuariosAsync(progreso);
            if (data.error != null) return data.error;

            var usuario = data.usuario!;
            var escaneado = data.escaneado!;

            if (!escaneado.Nombre.StartsWith("A", StringComparison.OrdinalIgnoreCase))
                return Error($"{escaneado.Nombre} no empieza por la letra A");

            return await CompletarAsync(usuario);
        }

        private async Task<ResultadoReto> VerificarVerdeMismaEdadAsync(ProgresoReto progreso)
        {
            var data = await ObtenerUsuariosAsync(progreso);
            if (data.error != null) return data.error;

            var usuario = data.usuario!;
            var escaneado = data.escaneado!;

            if (!escaneado.Sexo)
                return Error("La persona escaneada no es hombre");

            if (!MismoEstadoTexto(escaneado.Estado, "verde") &&
                !MismoEstadoTexto(escaneado.Estado, "soltero"))
                return Error("La persona escaneada no está en estado verde");

            if (CalcularEdad(usuario.FechaNacimiento) != CalcularEdad(escaneado.FechaNacimiento))
                return Error("La persona escaneada no tiene tu misma edad");

            return await CompletarAsync(usuario);
        }

        private async Task<ResultadoReto> VerificarAmarilloAsync(ProgresoReto progreso)
        {
            var data = await ObtenerUsuariosAsync(progreso);
            if (data.error != null) return data.error;

            var usuario = data.usuario!;
            var escaneado = data.escaneado!;

            if (!MismoEstadoTexto(escaneado.Estado, "amarillo") &&
                !MismoEstadoTexto(escaneado.Estado, "amigos") &&
                !MismoEstadoTexto(escaneado.Estado, "buscando amigos"))
                return Error("La persona escaneada no está en estado amarillo");

            return await CompletarAsync(usuario);
        }

        private async Task<ResultadoReto> VerificarMatch0PtsAsync(ProgresoReto progreso)
        {
            var data = await ObtenerUsuariosAsync(progreso);
            if (data.error != null) return data.error;

            var usuario = data.usuario!;
            var escaneado = data.escaneado!;

            if (escaneado.Puntos != 0)
                return Error($"{escaneado.Nombre} tiene {escaneado.Puntos} puntos, necesita tener 0");

            return await CompletarAsync(usuario);
        }

        private async Task<ResultadoReto> VerificarMismaEdadMismoEstadoAsync(ProgresoReto progreso)
        {
            var data = await ObtenerUsuariosAsync(progreso);
            if (data.error != null) return data.error;

            var usuario = data.usuario!;
            var escaneado = data.escaneado!;

            if (CalcularEdad(usuario.FechaNacimiento) != CalcularEdad(escaneado.FechaNacimiento))
                return Error("La persona escaneada no tiene tu misma edad");

            if (!MismoEstadoTexto(usuario.Estado, escaneado.Estado))
                return Error("La persona escaneada no tiene tu mismo estado");

            return await CompletarAsync(usuario);
        }

        private async Task<ResultadoReto> VerificarMismoEstadoAsync(ProgresoReto progreso)
        {
            var data = await ObtenerUsuariosAsync(progreso);
            if (data.error != null) return data.error;

            var usuario = data.usuario!;
            var escaneado = data.escaneado!;

            if (!MismoEstadoTexto(usuario.Estado, escaneado.Estado))
                return Error("La persona escaneada no tiene tu mismo estado");

            return await CompletarAsync(usuario);
        }

        private async Task<ResultadoReto> VerificarMasJovenAsync(ProgresoReto progreso)
        {
            var data = await ObtenerUsuariosAsync(progreso);
            if (data.error != null) return data.error;

            var usuario = data.usuario!;
            var escaneado = data.escaneado!;

            var usuariosSala = await _usuarioRepository.ObtenerPorSalaAsync(progreso.SalaId);
            if (usuariosSala == null || usuariosSala.Count == 0)
                return Error("No se pudieron comprobar los usuarios de la sala");

            var fechaMasReciente = usuariosSala.Max(u => u.FechaNacimiento);

            if (escaneado.FechaNacimiento.Date != fechaMasReciente.Date)
                return Error("Esta persona no es la más joven de la sala");

            return await CompletarAsync(usuario);
        }

        private async Task<ResultadoReto> VerificarMismaEdadDiferenteEstadoAsync(ProgresoReto progreso)
        {
            var data = await ObtenerUsuariosAsync(progreso);
            if (data.error != null) return data.error;

            var usuario = data.usuario!;
            var escaneado = data.escaneado!;

            if (CalcularEdad(usuario.FechaNacimiento) != CalcularEdad(escaneado.FechaNacimiento))
                return Error("La persona escaneada no tiene tu misma edad");

            if (MismoEstadoTexto(usuario.Estado, escaneado.Estado))
                return Error("La persona escaneada tiene tu mismo estado, debe ser diferente");

            return await CompletarAsync(usuario);
        }

        private async Task<ResultadoReto> VerificarCualquierEscaneoAsync(ProgresoReto progreso)
        {
            var data = await ObtenerUsuariosAsync(progreso);
            if (data.error != null) return data.error;

            return await CompletarAsync(data.usuario!);
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.UtcNow;
            int edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }

        private bool MismoEstadoTexto(string? a, string? b)
        {
            return NormalizarEstado(a) == NormalizarEstado(b);
        }

        private string NormalizarEstado(string? estado)
        {
            var value = (estado ?? "").ToLower().Trim();

            if (value.Contains("soltero") || value.Contains("verde"))
                return "verde";

            if (value.Contains("amigo") || value.Contains("amarillo") || value.Contains("complicado"))
                return "amarillo";

            if (value.Contains("pareja") || value.Contains("rojo"))
                return "rojo";

            return value;
        }

        private async Task SumarPuntosFlashAsync(Usuario usuario, int puntos)
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

        private async Task<ResultadoReto> CompletarAsync(Usuario usuario)
        {
            await SumarPuntosFlashAsync(usuario, 30);
            return Exito("¡Reto completado! +30 puntos");
        }

        private ResultadoReto Exito(string mensaje) =>
            new ResultadoReto
            {
                Exito = true,
                Mensaje = mensaje,
                PuntosGanados = 30
            };

        private ResultadoReto Error(string mensaje) =>
            new ResultadoReto
            {
                Exito = false,
                Mensaje = mensaje
            };
    }
}