
using Microsoft.AspNetCore.Mvc;
using VittaMais.API.Models;
using VittaMais.API.Models.DTOs;
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

        [HttpPost("cadastrar-com-foto")]
        public async Task<IActionResult> CadastrarEspecialidadeComFoto([FromForm] EspecialidadeDTO dto)
        {
            try
            {
                var especialidade = new Especialidade
                {
                    Nome = dto.Nome,
                    Descricao = dto.Descricao,
                    Valor = dto.Valor
                    // ImagemUrl será preenchida no service
                };

                var id = await _especialidadeService.AdicionarEspecialidadeComImagemAsync(dto.Imagem, especialidade);

                return Ok(new
                {
                    mensagem = "Especialidade cadastrada com sucesso!",
                    id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    erro = ex.Message
                });
            }
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            var lista = await _especialidadeService.ListarEspecialidades();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(string id)
        {
            var especialidade = await _especialidadeService.ObterPorId(id);
            if (especialidade == null)
            {
                return NotFound(new { mensagem = "Especialidade não encontrada." });
            }
            return Ok(especialidade);
        }

        [HttpPut("editar-com-foto/{id}")]
        public async Task<IActionResult> EditarEspecialidadeComFoto(string id, [FromForm] EspecialidadeDTO dto)
        {
            try
            {
                var especialidade = new Especialidade
                {
                    Nome = dto.Nome,
                    Descricao = dto.Descricao,
                    Valor = dto.Valor,
                    Status = dto.Status
                    // ImagemUrl será setado no service se enviado
                };

                await _especialidadeService.EditarEspecialidadeComImagemAsync(id, dto.Imagem, especialidade);

                return Ok(new { mensagem = "Especialidade atualizada com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

    }
}
