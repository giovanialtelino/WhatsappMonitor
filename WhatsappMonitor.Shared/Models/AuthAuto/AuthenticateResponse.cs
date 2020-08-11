using System.Text.Json.Serialization;
using WhatsappMonitor.Shared.Models;

namespace WhatsappMonitor.Shared.Models.AuthAuto
{
    public class AuthenticateResponse
    {
        public int AuthenticateResposeId { get; set; }
        public string Username { get; set; }

        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
        public AuthenticateResponse(){}

        public AuthenticateResponse(User user, string jwtToken, string refreshToken)
        {
            AuthenticateResposeId = user.UserId;
            Username = user.Username;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }

        public AuthenticateResponse(int authenticateResposeId, string username, string jwtToken, string refreshToken)
        {
            AuthenticateResposeId = authenticateResposeId;
            Username = username;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }

}