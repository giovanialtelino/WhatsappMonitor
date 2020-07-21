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
        public int ChatId { get; set; }

        [Required]
        [DisplayName("Person Name")]
        [JsonPropertyName("personName")]
        public string PersonName { get; set; }

        [Required]
        [DisplayName("Message Time")]
        [JsonPropertyName("messageTime")]
        public DateTime MessageTime { get; set; }

        [Required]
        [DisplayName("System Time")]
        [JsonPropertyName("systemTime")]
        public DateTime SystemTime { get; set; }

        [Required]
        [JsonPropertyName("message")]
        public String Message { get; set; }

        public int? EntityId { get; set; }
        public Entity Group { get; set; }
        public Chat(){}

        public Chat(string personName, string message, DateTime messageTime)
        {
            PersonName = personName;
            Message = message;
            MessageTime = messageTime;
        }
        public Chat(string personName, DateTime messageTime, DateTime systemTime, string message)
        {
            PersonName = personName;
            MessageTime = messageTime;
            SystemTime = systemTime;
            Message = message;
        }
        public Chat(string personName, DateTime messageTime, DateTime systemTime, string message, int entityId)
        {
            PersonName = personName;
            MessageTime = messageTime;
            SystemTime = systemTime;
            Message = message;
            EntityId = entityId;
        }
    }
}