namespace VittaMais.API.Models
{
    public class ConsultaDetalhada
    {
        public string Id { get; set; }
        public string MedicoId { get; set; }
        public string MedicoNome { get; set; }
        public string PacienteId { get; set; }
        public string PacienteNome { get; set; }
        public string EspecialidadeId { get; set; }
        public string EspecialidadeNome { get; set; }
        public DateTime Data { get; set; }
        public StatusConsulta Status { get; set; }
        public string Diagnostico { get; set; }
        public string Observacoes { get; set; }
        public string Remedios { get; set; }
        public string Cpf { get; set; }
        public Endereco Endereco { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
