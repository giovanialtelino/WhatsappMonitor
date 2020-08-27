using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace WhatsappMonitor.Shared.Models
{
    public class ChatMessage
    {
        [Key]
        [JsonPropertyName("id")]
        public int ChatMessageId { get; set; }

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

        public int FolderId { get; set; }
        public Folder Folder { get; set; }
        public ChatMessage(){}

        public ChatMessage(string personName, string message, DateTime messageTime)
        {
            PersonName = personName;
            Message = message;
            MessageTime = messageTime;
        }
        public ChatMessage(string personName, DateTime messageTime, DateTime systemTime, string message)
        {
            PersonName = personName;
            MessageTime = messageTime;
            SystemTime = systemTime;
            Message = message;
        }
        public ChatMessage(string personName, DateTime messageTime, DateTime systemTime, string message, int entityId)
        {
            PersonName = personName;
            MessageTime = messageTime;
            SystemTime = systemTime;
            Message = message;
            FolderId = entityId;
        }
    }
}