using System.Text.Json.Serialization;
using WhatsappMonitor.Shared.Models;

namespace WhatsappMonitor.Shared.Models.AuthAuto
{
    public class AuthenticateResponse
    {
        public int AuthenticateResposeId { get; set; }
        public string Username { get; set; }

        public string JwtToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }

        public AuthenticateResponse(User user, string jwtToken, string refreshToken)
        {
            AuthenticateResposeId = user.UserId;
            Username = user.Username;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }

}