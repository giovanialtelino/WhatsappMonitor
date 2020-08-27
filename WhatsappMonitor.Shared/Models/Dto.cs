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
        public string NewName { get; set; }

        [Required]
        [DisplayName("First Message")]
        [JsonPropertyName("firstMessage")]
        public DateTime FirstMessage { get; set; }

        [Required]
        [DisplayName("Last Message")]
        [JsonPropertyName("lastMessage")]
        public DateTime LastMessage { get; set; }

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
        public decimal MessageCounterPercentage { get; set; }

        [Required]
        [DisplayName("Words Counter")]
        [JsonPropertyName("wordsCounterPercentage")]
        public decimal WordsCounterPercentage { get; set; }
        [DisplayName("To Delete")]
        [JsonPropertyName("toDelete")]
        public bool ToDelete { get; set; }
        public ParticipantDTO() { }

         public ParticipantDTO(string personName, DateTime firstMessage, DateTime lastMessage, int messageCounter, int wordsCounter)
        {
            PersonName = personName;
            NewName = personName;
            ToDelete = false;
            FirstMessage = firstMessage;
            LastMessage = lastMessage;
            MessageCounter = messageCounter;
            WordsCounter = wordsCounter;
        }
        public ParticipantDTO(string personName, DateTime firstMessage, DateTime lastMessage, int messageCounter, int wordsCounter, decimal messageCounterPercentage, decimal wordsCounterPercentage)
        {
            PersonName = personName;
            NewName = personName;
            ToDelete = false;
            FirstMessage = firstMessage;
            LastMessage = lastMessage;
            MessageCounter = messageCounter;
            WordsCounter = wordsCounter;
            MessageCounterPercentage = messageCounterPercentage;
            WordsCounterPercentage = wordsCounterPercentage;
        }

        public void AddMessageAndWordPercentage(decimal messageCounterPercentage, decimal wordsCounterPercentage)
        {
            this.MessageCounterPercentage = messageCounterPercentage;
            this.WordsCounterPercentage = wordsCounterPercentage;
        }
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

    public class PaginationDTO
    {
        public int Pagination { get; set; }
        public int Take { get; set; }
        public bool AllowNext { get; set; }
        public bool AllowBack { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public PaginationDTO(int pagination, int take, bool allowNext, bool allowBack, int totalPages, int currentPage)
        {
            Pagination = pagination;
            Take = take;
            AllowNext = allowNext;
            AllowBack = allowBack;
            TotalPages = totalPages;
            CurrentPage = currentPage;
        }
    }
}