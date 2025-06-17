using Firebase.Database.Query;
using Microsoft.AspNetCore.Mvc;
using VittaMais.API.Models;
using VittaMais.API.Models.DTOs;
using VittaMais.API.Services;

namespace VittaMais.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultasController : ControllerBase
    {
        private readonly ConsultaService _consultaService;
        private readonly FirebaseService _firebaseService;

        public ConsultasController(ConsultaService consultaService, FirebaseService firebaseService)
        {
            _consultaService = consultaService;
            _firebaseService = firebaseService;
        }



        [HttpPost("agendar-consulta")]
        public async Task<IActionResult> Post([FromBody] CriarConsultaDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { mensagem = "Dados da consulta inválidos." });
            }

            var novaConsulta = new Consulta
            {
                PacienteId = dto.PacienteId,
                NomePaciente = dto.NomePaciente,
                MedicoId = dto.MedicoId,
                Data = dto.Data,
                Status = dto.Status,
                EspecialidadeId = dto.EspecialidadeId
            };

            try
            {
                await _consultaService.AgendarConsulta(novaConsulta);

                // Retorna status 200 com mensagem de sucesso
                return Ok(new
                {
                    mensagem = "Consulta agendada com sucesso.",
                    consulta = novaConsulta
                });
            }
            catch (Exception ex)
            {
                // Retorna status 500 com mensagem de erro
                return StatusCode(500, new
                {
                    mensagem = "Erro ao agendar a consulta.",
                    erro = ex.Message
                });
            }
        }

        [HttpPut("Atualizar-consulta")]
        public async Task<IActionResult> AtualizarConsulta([FromBody] AtualizarConsultaDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Id))
            {
                return BadRequest(new { mensagem = "Dados de atualização inválidos." });
            }

            try
            {
                await _consultaService.AtualizarConsulta(dto);
                return Ok(new { mensagem = "Consulta atualizada com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao atualizar consulta.", erro = ex.Message });
            }
        }

        [HttpGet("listar-por-especialidade/{especialidadeId}")]
        public async Task<IActionResult> ListarPorEspecialidade(string especialidadeId)
        {
            var consultas = await _consultaService.ListarConsultasPorEspecialidade(especialidadeId);
            return Ok(consultas);
        }


        [HttpGet("listar-por-medico/{medicoId}")]
        public async Task<IActionResult> ListarPorMedico(string medicoId)
        {
            // Chama o serviço para listar as consultas do médico
            var consultas = await _consultaService.ListarConsultasPorMedico(medicoId);

            // Se não encontrar nenhuma consulta, retorna uma mensagem informando isso
            if (consultas == null || consultas.Count == 0)
            {
                return NotFound(new { Mensagem = "Nenhuma consulta encontrada para este médico." });
            }

            // Caso haja consultas, retorna elas no formato OK (200)
            return Ok(consultas);
        }


        [HttpGet("listar-detalhada-por-especialidade/{especialidadeId}")]
        public async Task<IActionResult> ListarConsultasDetalhadas(string especialidadeId)
        {
            var consultas = await _consultaService.ListarConsultasDetalhadasPorEspecialidade(especialidadeId);
            return Ok(consultas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConsultaPorId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { mensagem = "ID da consulta é obrigatório." });

            try
            {
                var consulta = await _consultaService.ObterConsultaPorId(id);

                if (consulta == null)
                    return NotFound(new { mensagem = "Consulta não encontrada." });

                return Ok(consulta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao buscar a consulta.", erro = ex.Message });
            }
        }


        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObterConsultasPorUsuario(string usuarioId)
        {
            var resultado = await _consultaService.ObterConsultasPorUsuario(usuarioId);

            // Se o resultado for uma mensagem de erro (usuário não encontrado)
            if (resultado is not List<object>)
            {
                return NotFound(resultado);
            }

            return Ok(resultado);
        }

        [HttpGet("listar-consultas-por-data")]
        public async Task<ActionResult<List<Consulta>>> GetConsultasPorData([FromQuery] DateTime data)
        {
            var consultas = await _consultaService.ListarConsultasPorData(data);
            return Ok(consultas);
        }

    }
}
