using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUser = "vivabemclinicamed@gmail.com"; // seu email do Gmail
    private readonly string _smtpPass = "kaxf cmry pfix ltof"; // a senha de app

    public async Task EnviarEmailConsultaAsync(string nomePaciente, string emailPaciente, string especialidade, string medico, DateTime dataConsulta)
    {
        var mensagem = new MimeMessage();

        mensagem.From.Add(new MailboxAddress("VivaBem", _smtpUser));
        mensagem.To.Add(new MailboxAddress(nomePaciente, emailPaciente));
        mensagem.Subject = "Confirmação de Consulta - VivaBem";

        mensagem.Body = new TextPart("html")
        {
            Text = $@"
        <!DOCTYPE html>
        <html lang=""pt-BR"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #F5F7FA;
                    color: #2C3E50;
                    margin: 0;
                    padding: 20px;
                }}
                .container {{
                    max-width: 600px;
                    margin: 0 auto;
                    background-color: #fff;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
                }}
                h1 {{
                    color: #007B8A;
                    font-size: 24px;
                    margin-bottom: 10px;
                }}
                h3 {{
                    color: #00B894;
                    margin-top: 30px;
                }}
                p {{
                    font-size: 16px;
                    line-height: 1.5;
                }}
                strong {{
                    color: #007B8A;
                }}
                .footer {{
                    margin-top: 40px;
                    font-size: 12px;
                    color: #A2DED0;
                    text-align: center;
                }}
            </style>
        </head>
        <body>
            <div class=""container"">
                <h1>Olá, {nomePaciente}!</h1>
                <p>Estamos enviando este e-mail para confirmar a sua consulta.</p>
                <h3>Detalhes da Consulta</h3>
                <p><strong>Paciente:</strong> {nomePaciente}</p>
                <p><strong>Data:</strong> {dataConsulta:dd/MM/yyyy HH:mm}</p>
                <p><strong>Especialidade:</strong> {especialidade}</p>
                <p><strong>Médico:</strong> {medico}</p>
                <p><strong>Aguardamos você!</strong></p>
                <div class=""footer"">
                    VivaBem Clínica Médica &copy; 2025
                </div>
            </div>
        </body>
        </html>"
        };

        using var cliente = new SmtpClient();

        await cliente.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await cliente.AuthenticateAsync(_smtpUser, _smtpPass);
        await cliente.SendAsync(mensagem);
        await cliente.DisconnectAsync(true);
    }

}
