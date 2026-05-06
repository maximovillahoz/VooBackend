using Microsoft.AspNetCore.Mvc;

namespace VooApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PreguntasController : ControllerBase
{
    private static readonly Dictionary<string, List<string>> Preguntas = new()
    {
        ["verde"] = new List<string>
        {
            "¿Prefieres que te roben un beso o ser tú quien lo robe?",
            "¿Cita ideal: Cena romántica o fiesta descontrolada?",
            "¿Team Pizza con piña o team \"eso es un pecado\"?"
        },
        ["amarillo"] = new List<string>
        {
            "¿En el grupo eres el que siempre baila o el que se queda en la barra?",
            "¿Cuál es tu guilty pleasure musical?",
            "¿Atrevido/a o tímido/a al conocer gente nueva?"
        },
        ["rojo"] = new List<string>
        {
            "¿Cómo va la relación: Luna de miel o ya somos como un viejo matrimonio?",
            "¿Monogamia clásica o mente abierta?",
            "¿Quién es más probable que termine en el suelo riendo hoy?"
        }
    };

    [HttpGet("{estado}")]
    public IActionResult GetPreguntas(string estado)
    {
        estado = estado.ToLower();
        if (!Preguntas.ContainsKey(estado))
            return BadRequest(new { mensaje = "Estado no válido. Usa: verde, amarillo o rojo" });

        return Ok(Preguntas[estado]);
    }
}