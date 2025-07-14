namespace VittaMais.API.Models.DTOs
{
    public class AtualizarConsultaDto
    {
        public string Id { get; set; }

        public string? Diagnostico { get; set; }
        public string? Observacoes { get; set; }
        public string? Remedios { get; set; }
        public string? SintomasPaciente { get; set; }
        public string? RelatoriosMedicos { get; set; }
        public string? RemediosDiarios { get; set; }
        public string? ProblemasSaude { get; set; }
        public StatusConsulta? Status { get; set; }
    }

}
