using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace WhatsappMonitor.Shared.Models
{
    public class Chat
    {
        [Key]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [DisplayName("Person Name")]
        [JsonPropertyName("personName")]
        public string PersonName { get; set; }

        [Required]
        [DisplayName("Message Time")]
        [JsonPropertyName("messageTime")]
        public DateTime MessageTime { get; set; }

        [Required]
        [JsonPropertyName("message")]
        public String Message { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
    }
}