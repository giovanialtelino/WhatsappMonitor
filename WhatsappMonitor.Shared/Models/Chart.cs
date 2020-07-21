using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace WhatsappMonitor.Shared.Models
{
    public class ChartDTO
    {

    }

    public class ChatPersonInfoDTO
    {
        public string MessageCounter { get; set; }
        public string MessagePercentage { get; set; }
        public string WordsCounter { get; set; }
        public string WordsPercentage { get; set; }
        public List<string> CommonWords { get; set; }
        public List<Tuple<string, string>> Hours { get; set; }
    }

    public class TotalFolderInfoDTO
    {
        public string TotalMessage { get; set; }
        public string TotalWords { get; set; }
        public List<Tuple<string, int>> CommonWords { get; set; }
        public List<Tuple<string, int>> CommonHours { get; set; }
    }
}