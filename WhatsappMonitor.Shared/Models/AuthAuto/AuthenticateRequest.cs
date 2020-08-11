using System.ComponentModel.DataAnnotations;

namespace WhatsappMonitor.Shared.Models.AuthAuto
{
    public class AuthenticateRequest
    {
        [Required]
        [MinLength(3, ErrorMessage = "Username is too short.")]
        public string Username { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Password is too short.")]
        public string Password { get; set; }
    }
}