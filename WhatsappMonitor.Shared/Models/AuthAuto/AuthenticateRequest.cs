using System.ComponentModel.DataAnnotations;

namespace WhatsappMonitor.Shared.Models.AuthAuto
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}