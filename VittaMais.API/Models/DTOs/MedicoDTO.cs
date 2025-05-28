using System.ComponentModel.DataAnnotations;

namespace VittaMais.API.Models.DTOs
{
    public class MedicoDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email informado não é válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 16 caracteres.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A especialidade é obrigatória.")]
        public string EspecialidadeId { get; set; }

        // Campos opcionais (não obrigatório, sem [Required])
        [StringLength(14, ErrorMessage = "CPF deve ter até 14 caracteres.")]
        public string? Cpf { get; set; }

        public DateTime? DataNascimento { get; set; }

        [StringLength(20, ErrorMessage = "Telefone deve ter até 20 caracteres.")]
        public string? Telefone { get; set; }

        public Endereco Endereco { get; set; }
    }
}
