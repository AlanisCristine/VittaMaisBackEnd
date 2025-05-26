using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using VittaMais.API.Models;
using VittaMais.API.Models.DTOs;

using VittaMais.API.Services;

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

        [HttpPost("cadastrar/paciente")]
        public async Task<IActionResult> CadastrarPaciente([FromBody] PacienteDTO pacienteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paciente = new Usuario
            {
                Nome = pacienteDto.Nome,
                Email = pacienteDto.Email,
                Senha = pacienteDto.Senha,
                Tipo = TipoUsuario.Paciente,
                Cpf = pacienteDto.Cpf,
                DataNascimento = pacienteDto.DataNascimento,
                Telefone = pacienteDto.Telefone,
                Endereco = pacienteDto.Endereco
            };

            var id = await _usuarioService.CadastrarUsuario(paciente);
            return Ok(new { Message = "Paciente cadastrado com sucesso!", UsuarioId = id });
        }

        [HttpPut("editar/paciente/{id}")]
        public async Task<IActionResult> EditarPaciente(string id, [FromBody] PacienteDTO pacienteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var atualizado = await _usuarioService.EditarPaciente(id, pacienteDto);

            if (!atualizado)
                return NotFound("Paciente não encontrado ou não é um paciente.");

            return Ok(new { Message = "Dados do paciente atualizados com sucesso!" });
        }



        [HttpPost("cadastrar/medico")]
        public async Task<IActionResult> CadastrarMedico([FromBody] MedicoDTO medicoDto)
        {
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

        [HttpGet("listar")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _usuarioService.ListarUsuarios();
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
    }
}
