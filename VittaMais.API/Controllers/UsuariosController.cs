﻿using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using VittaMais.API.Models;
using VittaMais.API.Models.DTOs;

using VittaMais.API.Services;
using VittaMais.API.Utils;

namespace VittaMais.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }


        [HttpPost("cadastrar")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] Usuario usuario)
        {
            // Valida se os dados do usuário são válidos
            if (usuario == null || string.IsNullOrEmpty(usuario.Email) || string.IsNullOrEmpty(usuario.Senha))
            {
                return BadRequest("Dados do usuário inválidos. Por favor, preencha todos os campos.");
            }

            if (!ValidacaoUtils.ValidarCPF(usuario.Cpf))
            {
                return BadRequest(new { mensagem = "CPF inválido. Verifique e tente novamente." });
            }

            try
            {
                // Tenta cadastrar o usuário
                var id = await _usuarioService.CadastrarUsuario(usuario);

                if (string.IsNullOrEmpty(id))
                {
                    return StatusCode(500, "Erro ao tentar cadastrar o usuário. Tente novamente.");
                }

                // Sucesso no cadastro
                return Ok(new { Message = "Usuário cadastrado com sucesso!", UsuarioId = id });
            }
            catch (Exception ex)
            {
                // Retornar erro genérico para o front-end
                return StatusCode(500, "Erro ao tentar cadastrar. Tente novamente.");
            }
        }


        [HttpPost("cadastrar-paciente-com-foto")]
        public async Task<IActionResult> CadastrarPacienteComFoto([FromForm] PacienteDTO dto)
        {
            if (!ValidacaoUtils.ValidarCPF(dto.Cpf))
            {
                return BadRequest(new { mensagem = "CPF inválido. Verifique e tente novamente." });
            }
            try
            {
                var novoPaciente = new Usuario
                {
                    Nome = dto.Nome,
                    Email = dto.Email,
                    Senha = dto.Senha,
                    Telefone = dto.Telefone,
                    Cpf = dto.Cpf,
                    DataNascimento = dto.DataNascimento,
                    Endereco = dto.Endereco,
                    Tipo = TipoUsuario.Paciente
                };

                var id = await _usuarioService.CadastrarUsuarioComFoto(dto.FotoPerfil, novoPaciente);
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPost("cadastrar-medico-com-foto")]
        public async Task<IActionResult> CadastrarMédicoComFoto([FromForm] MedicoDTO dto)
        {
            if (!dto.DataNascimento.HasValue)
                return BadRequest(new { erro = "Data de nascimento é obrigatória." });

            if (!ValidacaoUtils.ValidarCPF(dto.Cpf))
            {
                return BadRequest(new { mensagem = "CPF inválido. Verifique e tente novamente." });
            }
            try
            {
                var novoMedico = new Usuario
                {
                    Nome = dto.Nome,
                    Email = dto.Email,
                    Senha = dto.Senha,
                    EspecialidadeId = dto.EspecialidadeId,
                    Telefone = dto.Telefone,
                    Cpf = dto.Cpf,
                    DataNascimento = dto.DataNascimento.Value,
                    Endereco = dto.Endereco,
                    Tipo = TipoUsuario.Medico
                }; 

                var id = await _usuarioService.CadastrarUsuarioComFoto(dto.FotoPerfil, novoMedico);
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPost("cadastrar/medico")]
        public async Task<IActionResult> CadastrarMedico([FromBody] MedicoDTO medicoDto)
        {
            if (!ValidacaoUtils.ValidarCPF(medicoDto.Cpf))
            {
                return BadRequest(new { mensagem = "CPF inválido. Verifique e tente novamente." });
            }
            var medico = new Usuario
            {
                Nome = medicoDto.Nome,
                Email = medicoDto.Email,
                Senha = medicoDto.Senha,
                Tipo = TipoUsuario.Medico,
                EspecialidadeId = medicoDto.EspecialidadeId
            };

            var id = await _usuarioService.CadastrarUsuario(medico);
            return Ok(new { Message = "Médico cadastrado com sucesso!", UsuarioId = id });
        }


        [HttpPost("cadastrar/diretor")]
        public async Task<IActionResult> CadastrarDiretor([FromBody] DiretorDTO diretorDto)
        {
            var diretor = new Usuario
            {
                Nome = diretorDto.Nome,
                Email = diretorDto.Email,
                Senha = diretorDto.Senha,
                Tipo = TipoUsuario.Diretor
            };

            var id = await _usuarioService.CadastrarUsuario(diretor);
            return Ok(new { Message = "Diretor cadastrado com sucesso!", UsuarioId = id });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Models.DTOs.LoginRequest request)
        {
            var usuario = await _usuarioService.Login(request.Email, request.Senha);

            if (usuario == null)
                return Unauthorized("E-mail ou senha inválidos.");

            return Ok(new { Message = "Login bem-sucedido!", Usuario = usuario });
        }

        [HttpPut("Atualizar-medico-pelo/{id}")]
        public async Task<IActionResult> AtualizarDadosBasicosMedico(string id, [FromBody] AtualizarDadosMedicoDTO dto)
        {
            try
            {
                await _usuarioService.AtualizarDadosBasicosMedicoAsync(id, dto);
                return Ok(new { mensagem = "Dados atualizados com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao atualizar dados.", erro = ex.Message });
            }
        }

        [HttpPut("Atualizar-Diretor-pelo/{id}")]
        public async Task<IActionResult> AtualizarDadosBasicosDiretor(string id, [FromBody] AtualizarDadosDiretorDTO dto)
        {
            try
            {
                await _usuarioService.AtualizarDadosBasicosDiretorAsync(id, dto);
                return Ok(new { mensagem = "Dados atualizados com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao atualizar dados.", erro = ex.Message });
            }
        }

        [HttpPut("Atualizar-Foto-Paciente/{id}")]
        public async Task<IActionResult> AtualizarFotoPerfil(string id, IFormFile fotoPerfil)
        {
            try
            {
                await _usuarioService.AtualizarFotoPerfilAsync(id, fotoPerfil);
                return Ok(new { mensagem = "Foto de perfil atualizada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

       

        [HttpPut("Atualizar-paciente-pelo/{id}")]
        public async Task<IActionResult> AtualizarDadosBasicos(string id, [FromBody] AtualizarDadosPacienteDto dto)
        {
            try
            {
                await _usuarioService.AtualizarDadosBasicosAsync(id, dto);
                return Ok(new { mensagem = "Dados atualizados com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao atualizar dados.", erro = ex.Message });
            }
        }


        [HttpPut("Inativar-Medico/{id}")]
        public async Task<IActionResult> InativarMedico(string id)
        {
            var medico = await _usuarioService.BuscarPorIdAsync(id);
            if (medico == null) return NotFound("Médico não encontrado.");

            medico.EstaAtuando = false;
            medico.DataSaida = DateTime.UtcNow;

            await _usuarioService.AtualizarAsync(id, medico);
            return Ok("Médico inativado com sucesso.");
        }


        [HttpGet("listar")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _usuarioService.ListarUsuarios();
            return Ok(usuarios);
        }

        [HttpGet("Listar-por/{id}")]
        public async Task<IActionResult> GetUsuarioById(string id)
        {
            var usuario = await _usuarioService.BuscarPorIdAsync(id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }


        [HttpGet("listar-usuarios-por-email")]
        public async Task<IActionResult> ListarUsuariosPorEmail( string email)
        {
            var usuarios = await _usuarioService.BuscarUsuarioPorEmail(email);
            return Ok(usuarios);
        }

        [HttpGet("listar-medicos")]
        public async Task<IActionResult> ListarMedicos()
        {
            var medicos = await _usuarioService.ListarMedicos();

            if (medicos == null || !medicos.Any())
            {
                return NotFound("Nenhum médico encontrado.");
            }

            return Ok(medicos);
        }


       [HttpGet("listar-Pacientes")]
       public async Task<IActionResult> ListarPacientes()
        {
            var pacientes = await _usuarioService.ListarPacientes();

            if (pacientes == null || !pacientes.Any())
            {
                return NotFound("Nenhum médico encontrado.");
            }

            return Ok(pacientes);
        }

        //[HttpPost("recuperar-senha")]
        //public async Task<IActionResult> RecuperarSenha([FromBody] RecuperarSenhaRequestDto dto)
        //{
        //    var sucesso = await _usuarioService.EnviarTokenRecuperacaoAsync(dto.Email);
        //    if (!sucesso)
        //        return BadRequest(new { mensagem = "Não foi possível enviar o email de recuperação." });

        //    return Ok(new { mensagem = "Token de recuperação enviado com sucesso." });
        //}

    }
}
