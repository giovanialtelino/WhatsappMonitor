using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace WhatsappMonitor.Shared.Models
{
    public class Entity
    {
        [Key]
        [JsonPropertyName("id")]
        public int EntityId { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Creation Date")]
        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }

        public List<Chat> Chats {get;set;}
        public Entity(string name)
        {
            Name = name;
            CreationDate = DateTime.Now;
        }
    }
}