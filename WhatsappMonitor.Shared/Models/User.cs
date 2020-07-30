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
        public RefreshToken RefreshToken { get; set; }
    }

    public class RefreshToken
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
    }


}