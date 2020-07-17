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
}