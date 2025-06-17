using System.Text.Json.Serialization;

namespace VittaMais.API.Models
{
    public class Consulta
    {
        public string Id { get; set; }
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

    public enum StatusConsulta
    {
        Agendada = 1,
        Realizada = 2,
        Cancelada = 3,
        Pendente = 4
    }

}
