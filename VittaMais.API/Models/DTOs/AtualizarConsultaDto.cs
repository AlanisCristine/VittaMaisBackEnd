namespace VittaMais.API.Models.DTOs
{
    public class AtualizarConsultaDto
    {
        public string Id { get; set; }

        public string Diagnostico { get; set; }
        public string Observacoes { get; set; }
        public string Remedios { get; set; }
    }
}
