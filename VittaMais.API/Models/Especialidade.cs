namespace VittaMais.API.Models
{
    public class Especialidade
    {
        public string Id { get; set; } 
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string? ImagemUrl { get; set; }
        public decimal Valor { get; set; }
        public bool Status { get; set; } = true; // Começa como ativa
    }
}
