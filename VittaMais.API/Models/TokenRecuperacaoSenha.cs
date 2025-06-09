namespace VittaMais.API.Models
{
    public class TokenRecuperacaoSenha
    {
        public string Token { get; set; }
        public string UsuarioId { get; set; }
        public DateTime CriadoEm { get; set; }
        public bool Usado { get; set; }
    }

}
