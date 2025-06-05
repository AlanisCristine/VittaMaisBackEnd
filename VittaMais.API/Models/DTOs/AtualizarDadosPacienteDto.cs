namespace VittaMais.API.Models.DTOs
{
    public class AtualizarDadosPacienteDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public Endereco Endereco { get; set; }
        public string Cpf { get; set; }
        public string Telefone { get; set; }
        public DateTime DataNascimento { get; set; }
    }

}
