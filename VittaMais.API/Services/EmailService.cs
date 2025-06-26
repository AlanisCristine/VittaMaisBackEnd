using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VittaMais.API; // ou use: using Task = System.Threading.Tasks.Task;

public class EmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public EmailService(IOptions<BrevoSettings> brevoSettings)
    {
        _apiKey = brevoSettings.Value.ApiKey;

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("api-key", _apiKey);
    }

    public async Task EnviarEmailConsultaAsync(string emailPaciente, string nomePaciente, string especialidade, string medico, DateTime dataConsulta)
    {
        var conteudoHtml = $@"
        <h1><strong>Olá </strong>{nomePaciente}</h1>
        <p>Estamos enviando esse email para a confirmação da consulta.</p>
        <h3>Confirmação de Consulta</h3>
        <p><strong>Paciente:</strong> {nomePaciente}</p>
        <p><strong>Data:</strong> {dataConsulta:dd/MM/yyyy HH:mm}</p>
        <p><strong>Especialidade:</strong> {especialidade}</p>
        <p><strong>Médico:</strong> {medico}</p>
        <p><strong>Aguardamos você!</strong></p>
    ";

        var emailObj = new
        {
            sender = new { name = "VivaBem", email = "vivabemclinicamed@gmail.com" },
            to = new[] { new { email = emailPaciente, name = nomePaciente } },
            subject = "Confirmação de Consulta - VivaBem",
            htmlContent = conteudoHtml
        };

        var json = JsonSerializer.Serialize(emailObj);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", content);

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao enviar email: {erro}");
        }
    }

}
