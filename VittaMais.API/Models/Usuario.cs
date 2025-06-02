using Newtonsoft.Json;

namespace VittaMais.API.Models
{
    public enum TipoUsuario
    {
        Paciente = 0,
        Medico = 1,
        Diretor = 2,
        Deslogado = 3,
    }

    public class Usuario
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("senha")]
        public string Senha { get; set; }

        [JsonProperty("tipo")]
        public TipoUsuario Tipo { get; set; }

        [JsonProperty("especialidadeId")]
        public string? EspecialidadeId { get; set; }

        [JsonProperty("cpf")]
        public string Cpf { get; set; }

        [JsonProperty("dataNascimento")]
        public DateTime DataNascimento { get; set; }

        [JsonProperty("telefone")]
        public string? Telefone { get; set; }

        [JsonProperty("endereco")]
        public Endereco Endereco { get; set; }

        [JsonProperty("fotoUrl")]
        public string? FotoUrl { get; set; }



    }
}
