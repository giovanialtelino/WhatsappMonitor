using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace WhatsappMonitor.Shared.Models
{
    public class Folder
    {
        [Key]
        [JsonPropertyName("id")]
        public int FolderId { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Creation Date")]
        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }
        public List<ChatMessage> FolderMessages { get; set; }
        public List<Upload> Uploads { get; set; }

        public Folder(string name)
        {
            Name = name;
            CreationDate = DateTime.Now;
            FolderMessages = new List<ChatMessage>();
            Uploads = new List<Upload>();
        }
    }
}