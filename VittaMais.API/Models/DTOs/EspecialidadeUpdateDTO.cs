namespace VittaMais.API.Models.DTOs
{
    public class EspecialidadeUpdateDTO
    {
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal? Valor { get; set; }
        public IFormFile? Imagem { get; set; }
        public bool Status { get; set; } = true;
    }
}
