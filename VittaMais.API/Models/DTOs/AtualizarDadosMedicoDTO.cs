using System.ComponentModel.DataAnnotations;

namespace VittaMais.API.Models.DTOs
{
    public class AtualizarDadosMedicoDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string? Cpf { get; set; }
        [Required]
        public DateTime? DataNascimento { get; set; }
        public string? Telefone { get; set; }
        public Endereco Endereco { get; set; }
    }
}
