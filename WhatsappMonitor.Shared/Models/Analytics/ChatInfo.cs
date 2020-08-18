using System;

namespace WhatsappMonitor.Shared.Models.Analytics
{
    public class ChatInfo
    {
        public int ChatInfoId { get; set; }
        public DateTime FirstMessage { get; set; }
        public DateTime LastMessage { get; set; }
        public int MessageCount { get; set; }
        public int WordCount { get; set; }
        public decimal MessagePercentage { get; set; }
        public decimal WordPercentage { get; set; }
        public Entity Entity { get; set; }
        public int EntityId { get; set; }
    }
}