using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using VittaMais.API.Models.DTOs;

namespace VittaMais.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContatoController : ControllerBase
    {
        [HttpPost("enviar")]
        public IActionResult EnviarMensagem([FromBody] ContatoDto contato)
        {
            if (contato == null || string.IsNullOrWhiteSpace(contato.Email) || string.IsNullOrWhiteSpace(contato.Mensagem))
                return BadRequest("Dados inválidos.");

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(contato.Nome, contato.Email));
                message.To.Add(new MailboxAddress("Clínica Viva Bem", "vivabemclinicamed@gmail.com"));
                message.Subject = $"Nova mensagem de contato - {contato.Nome}";

                message.Body = new TextPart("plain")
                {
                    Text = $"📧 Mensagem enviada pelo site VivaBem:\n\n" +
                           $"👤 Nome: {contato.Nome}\n" +
                           $"✉️ Email: {contato.Email}\n\n" +
                           $"💬 Mensagem:\n{contato.Mensagem}"
                };

                using (var client = new SmtpClient())
                {
                    // Para Gmail:
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate("vivabemclinicamed@gmail.com", "kaxf cmry pfix ltof");
                    client.Send(message);
                    client.Disconnect(true);
                }

                return Ok(new { mensagem = "Mensagem enviada com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar o email: {ex.Message}");
            }
        }

        [HttpGet("Listar-TESTE")]
        public async Task<string> ListarTESTE()
        {
            return "teste";
        }

        [HttpGet("Listar-TESTEdo teste")]
        public async Task<string> ListarTESTEdoTeste()
        {
            return "testeDoTeste";
        }
    }
}
