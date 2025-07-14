using Swashbuckle.AspNetCore.Annotations;

namespace VittaMais.API.Models.DTOs
{
    public class CriarConsultaDto
    {
        public string PacienteId { get; set; }
        public string EmailPaciente { get; set; }
        public string NomePaciente { get; set; }
        public string MedicoId { get; set; }
        public DateTime Data { get; set; }
        public StatusConsulta Status { get; set; }
        public string EspecialidadeId { get; set; }
        public string Diagnostico { get; set; } = "";
        public string Observacoes { get; set; } = "";
        public string Remedios { get; set; } = "";
        public string SintomasPaciente { get; set; } = "";
        public string RelatoriosMedicos { get; set; } = "";
        public string RemediosDiarios { get; set; } = "";
        public string ProblemasSaude { get; set; } = "";
    }
}
