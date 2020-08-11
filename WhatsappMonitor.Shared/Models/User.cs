using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WhatsappMonitor.Shared.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public List<RefreshToken> RefreshToken { get; set; }
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public User(string username, string password, RefreshToken refreshToken)
        {
            Username = username;
            Password = password;
            RefreshToken.Add(refreshToken);
        }
    }

    public class RefreshToken
    {
        [JsonIgnore]
        public int RefreshTokenId { get; set; }
        public string Token { get; set; }
        public DateTime Created { get; set; }
        public bool Valid { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }

        public RefreshToken(string token, DateTime created)
        {
            Token = token;
            Created = created;
            Valid = true;
        }

        public void MakeTokenInvalid()
        {
            this.Valid = false;
        }
    }
}