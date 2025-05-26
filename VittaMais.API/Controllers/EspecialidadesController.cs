using Microsoft.AspNetCore.Mvc;
using VittaMais.API.Models;
using VittaMais.API.Services;

namespace VittaMais.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EspecialidadesController : ControllerBase
    {
        private readonly EspecialidadeService _especialidadeService;

        public EspecialidadesController(EspecialidadeService especialidadeService)
        {
            _especialidadeService = especialidadeService;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] Especialidade especialidade)
        {
            var id = await _especialidadeService.AdicionarEspecialidade(especialidade);
            return Ok(new { Message = "Especialidade cadastrada com sucesso!", Id = id });
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            var lista = await _especialidadeService.ListarEspecialidades();
            return Ok(lista);
        }
    }
}
