namespace VittaMais.API.Models.DTOs
{
    public class ConsultaPacienteDto
    {
        public string NomePaciente { get; set; }
        public DateTime Data { get; set; }
        public string NomeMedico { get; set; }
        public string Especialidade { get; set; }
        public string Status { get; set; }
        public string Diagnostico { get; set; }
        public string Observacoes { get; set; }
        public string Remedios { get; set; }
        public string ProblemasSaude { get; set; }
        public string RelatoriosMedicos { get; set; }
        public string SintomasPaciente { get; set; }
        public string RemediosDiarios { get; set; }
    }

}
