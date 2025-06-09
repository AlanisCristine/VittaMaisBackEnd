namespace VittaMais.API.Models
{
    public class TokenRecuperacao
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime Expiracao { get; set; }
        public bool Usado { get; set; }
    }
}
