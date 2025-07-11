namespace VittaMais.API.Models
{
    public class Especialidade
    {
        public string Id { get; set; }  // Para o caso de precisar de um ID único
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string? ImagemUrl { get; set; }
        public decimal Valor { get; set; }
    }
}
