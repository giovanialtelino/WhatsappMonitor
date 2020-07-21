using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace WhatsappMonitor.Shared.Models
{
    public class ParticipantDTO
    {
        [Required]
        [DisplayName("Person Name")]
        [JsonPropertyName("personName")]
        public string PersonName { get; set; }
        [Required]
        [JsonPropertyName("fixedName")]
        public string FixedName { get; set; }

        [Required]
        [DisplayName("First Message")]
        [JsonPropertyName("firstMessage")]
        public DateTime FirstMessage { get; set; }

        [Required]
        [DisplayName("Message Counter")]
        [JsonPropertyName("messageCounter")]
        public int MessageCounter { get; set; }

        [Required]
        [DisplayName("Words Counter")]
        [JsonPropertyName("wordsCounter")]
        public int WordsCounter { get; set; }

        [Required]
        [DisplayName("Message Counter")]
        [JsonPropertyName("messageCounterPercentage")]
        public int MessageCounterPercentage { get; set; }

        [Required]
        [DisplayName("Words Counter")]
        [JsonPropertyName("wordsCounterPercentage")]
        public int WordsCounterPercentage { get; set; }
    }

    public class ChatUploadDTO
    {
        [Required]
        [DisplayName("Upload Date")]
        [JsonPropertyName("uploadDate")]
        public DateTime UploadDate { get; set; }

        [Required]
        [DisplayName("Chat Count")]
        [JsonPropertyName("chatCount")]
        public int ChatCount { get; set; }
    }

    public class ChatInfoDate
    {
        public DateTime? From { get; set; }
        public DateTime? Until { get; set; }
    }
}