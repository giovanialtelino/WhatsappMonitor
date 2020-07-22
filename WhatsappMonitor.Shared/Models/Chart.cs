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
        public string PersonName {get;set;}
        public int MessageCounter { get; set; }
        public double MessagePercentage { get; set; }
        public int WordsCounter { get; set; }
        public double WordsPercentage { get; set; }
        public List<Tuple<string, int>> CommonWords { get; set; }
        public List<Tuple<string, int>> Hours { get; set; }
    }

    public class TotalFolderInfoDTO
    {
        public int TotalMessage { get; set; }
        public int TotalWords { get; set; }
        public List<Tuple<string, double>> CommonWords { get; set; }
        public List<Tuple<string, double>> CommonHours { get; set; }
    }
}