using brevo_csharp.Api;
using brevo_csharp.Model;
using BrevoConfiguration = brevo_csharp.Client.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VittaMais.API.Services
{
    public class EmailService
    {
        private readonly string _apiKey = "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";

        public async Task<bool> EnviarEmailAsync(string paraEmail, string assunto, string corpoHtml)
        {
            try
            {
                BrevoConfiguration.Default.ApiKey.Add("api-key", _apiKey);

                var apiInstance = new TransactionalEmailsApi();

                var email = new SendSmtpEmail
                {
                    Subject = assunto,
                    HtmlContent = corpoHtml,
                    Sender = new SendSmtpEmailSender
                    {
                        Name = "Viva Bem",
                        Email = "alanisalmeidads@gmail.com"
                    },
                    To = new List<SendSmtpEmailTo>
            {
                new SendSmtpEmailTo { Email = paraEmail }
            }
                };

                var response = await apiInstance.SendTransacEmailAsync(email);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar email: " + ex.Message);
                return false;
            }
        }
    }
}

