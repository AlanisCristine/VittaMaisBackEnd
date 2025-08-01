﻿using Firebase.Database.Query;
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
        public async Task<IActionResult> AgendarConsulta([FromBody] CriarConsultaDto dto)
        {
            if (dto == null)
                return BadRequest(new { mensagem = "Dados da consulta inválidos." });

            try
            {
                var consulta = new Consulta
                {
                    PacienteId = dto.PacienteId,
                    NomePaciente = dto.NomePaciente,
                    EmailPaciente = dto.EmailPaciente,
                    MedicoId = dto.MedicoId,
                    NomeMedico = dto.NomeMedico,
                    Data = dto.Data,
                    Status = dto.Status,
                    EspecialidadeId = dto.EspecialidadeId, // será sobrescrita pelo service com a do médico
                    Diagnostico = dto.Diagnostico,
                    Observacoes = dto.Observacoes,
                    Remedios = dto.Remedios,
                    SintomasPaciente = dto.SintomasPaciente,
                    RelatoriosMedicos = dto.RelatoriosMedicos,
                    RemediosDiarios = dto.RemediosDiarios,
                    ProblemasSaude = dto.ProblemasSaude
                };

                var id = await _consultaService.AgendarConsulta(consulta);
                return Ok(new { mensagem = "Consulta agendada com sucesso.", consultaId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
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
            try
            {
                var consultas = await _consultaService.ObterConsultasPorUsuario(usuarioId);

                if (consultas == null || !consultas.Any())
                {
                    return NotFound(new { mensagem = "Nenhuma consulta encontrada para este paciente." });
                }

                return Ok(consultas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensagem = "Erro ao buscar consultas do usuário.",
                    erro = ex.Message
                });
            }
        }


        [HttpGet("listar-consultas-por-data")]
        public async Task<ActionResult<List<Consulta>>> GetConsultasPorData([FromQuery] DateTime data)
        {
            var consultas = await _consultaService.ListarConsultasPorData(data);
            return Ok(consultas);
        }

    }
}
