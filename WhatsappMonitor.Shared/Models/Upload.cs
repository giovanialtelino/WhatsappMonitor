using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace WhatsappMonitor.Shared.Models
{
    public class Upload
    {
        [Key]
        [JsonPropertyName("id")]
        public int UploadId { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string FileName { get; set; }

        [Required]
        [DisplayName("Creation Date")]
        [JsonPropertyName("creationDate")]
        public DateTime CreationDate { get; set; }

        [Required]
        [DisplayName("File Content")]
        [JsonPropertyName("fileContent")]
        public Byte[] FileContent { get; set; }

       
        public Entity Entity { get; set; }
        public int EntityId { get; set; }
    }
}