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
        private readonly UsuarioService _usuarioService;
        private readonly EmailService _emailService;
        private readonly EspecialidadeService _especialidadeService;
        public ConsultasController(ConsultaService consultaService, FirebaseService firebaseService, UsuarioService usuarioService, EspecialidadeService especialidadeService, EmailService emailService)
        {
            _consultaService = consultaService;
            _firebaseService = firebaseService;
            _usuarioService = usuarioService;
            _especialidadeService = especialidadeService;
           _emailService = emailService;
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
                EmailPaciente = dto.EmailPaciente,
                MedicoId = dto.MedicoId,
                Data = dto.Data,
                Status = dto.Status,
                EspecialidadeId = dto.EspecialidadeId
            };

            try
            {
                // Salva a consulta no banco
                await _consultaService.AgendarConsulta(novaConsulta);

                // === Enviar e-mail após agendamento ===
                try
                {
                    // Se o DTO não tem o email do paciente, busque aqui:
                    string emailPaciente = dto.EmailPaciente;
                    if (string.IsNullOrEmpty(emailPaciente))
                    {
                        var paciente = await _usuarioService.BuscarPorIdAsync(dto.PacienteId);
                        emailPaciente = paciente?.Email ?? "";
                    }

                    // Buscar o médico para pegar nome (se quiser)
                    var medico = await _usuarioService.BuscarPorIdAsync(dto.MedicoId);

                    // Buscar especialidade para pegar nome
                    var especialidade = await _especialidadeService.ObterPorId(dto.EspecialidadeId);

                    Console.WriteLine($"➡ Enviando email para {dto.EmailPaciente}...");
                    await _emailService.EnviarEmailConsultaAsync(
                    dto.NomePaciente,
                    dto.EmailPaciente,
                    especialidade.Nome,
                    medico.Nome,
                    dto.Data
                    );
                }
                catch (Exception ex)
                {
                    // Log do erro, mas não bloqueia o retorno da API
                    Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
                }

                return Ok(new
                {
                    mensagem = "Consulta agendada com sucesso.",
                    consulta = novaConsulta
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensagem = "Erro ao agendar a consulta.",
                    erro = ex.Message
                });
            }
        }

        [HttpPost("testar-email")]
        public async Task<IActionResult> TestarEnvioEmail()
        {
            try
            {
                await _emailService.EnviarEmailConsultaAsync(
                    "Alanis Almeida", // Nome do paciente
                    "alanisalmeidads@gmail.com", // E-mail do paciente
                    "Cardiologia", // Especialidade
                    "Dr. João da Silva", // Médico
                    DateTime.Now.AddDays(1).AddHours(14) // Data da consulta
                );

                return Ok(new { mensagem = "E-mail enviado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensagem = "Erro ao enviar o e-mail de teste",
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

        [HttpGet("historico-paciente/{usuarioId}")]
        public async Task<IActionResult> ObterHistoricoPorPaciente(string usuarioId)
        {
            try
            {
                var resultado = await _consultaService.ObterConsultasPorUsuario(usuarioId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter histórico: {ex.Message}");
            }
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
