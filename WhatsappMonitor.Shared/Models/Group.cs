using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace WhatsappMonitor.Shared.Models
{
    public class Group
    {
        [Key]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Creation Date")]
        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }

        public List<User> Users { get; set; }
    }
}