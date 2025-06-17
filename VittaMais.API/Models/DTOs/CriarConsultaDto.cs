namespace VittaMais.API.Models.DTOs
{
    public class CriarConsultaDto
    {
        public string PacienteId { get; set; }
        public string NomePaciente { get; set; }
        public string MedicoId { get; set; }
        public DateTime Data { get; set; }
        public StatusConsulta Status { get; set; }
        public string EspecialidadeId { get; set; }
        public string Diagnostico { get; set; }
        public string Observacoes { get; set; }
        public string Remedios { get; set; }
    }
}
