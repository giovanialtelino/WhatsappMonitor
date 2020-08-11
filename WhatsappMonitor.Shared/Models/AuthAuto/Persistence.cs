namespace WhatsappMonitor.Shared.Models.AuthAuto
{
    public class JwtPersistent
    {
        public string Token { get; private set; }
        public JwtPersistent(string token)
        {
            Token = token;
        }
    }

    public class RefreshTokenPersistent
    {
        public string Token { get; private set; }
        public RefreshTokenPersistent(string token)
        {
            Token = token;
        }
    }
}