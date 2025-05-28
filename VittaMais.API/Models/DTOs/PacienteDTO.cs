using System.ComponentModel.DataAnnotations;

namespace VittaMais.API.Models.DTOs
{
    public class PacienteDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength =2, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email informado não é válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 16 caracteres.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter exatamente 11 dígitos.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "O telefone informado não é válido.")]
        public string Telefone { get; set; }
        public Endereco Endereco { get; set; }

        public string? FotoPerfilUrl { get; set; }
    }
}
