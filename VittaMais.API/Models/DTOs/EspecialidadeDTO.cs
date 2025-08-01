using System.ComponentModel.DataAnnotations;

namespace VittaMais.API.Models.DTOs
{
    public class EspecialidadeDTO
    {

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        public decimal Valor { get; set; }

        public IFormFile? Imagem { get; set; }

        public bool Status { get; set; } = true;

    }
}
