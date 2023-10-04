namespace JWTMedico.Models
{
    public class Medico
    {
        public string CRM { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
