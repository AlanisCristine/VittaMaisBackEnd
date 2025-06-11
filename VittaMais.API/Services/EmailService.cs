using brevo_csharp.Api;
using brevo_csharp.Model;
using BrevoConfiguration = brevo_csharp.Client.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using brevo_csharp.Client;

namespace VittaMais.API.Services
{
    public class EmailService
    {
        private readonly string _apiKey;

        public EmailService(IConfiguration configuration)
        {
            _apiKey = configuration["Brevo:ApiKey"];
            Console.WriteLine(">>> API KEY LIDA: " + _apiKey);
        }
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
            catch (brevo_csharp.Client.ApiException ex)
            {
                Console.WriteLine("❌ Erro completo:");
                Console.WriteLine(ex.ToString());
                return false;
            }


        }
    }
}

