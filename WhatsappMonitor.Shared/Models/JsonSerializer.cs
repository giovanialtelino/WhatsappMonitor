using System;

//Json to class, used to convert a JSON file.
namespace WhatsappMonitor.Shared.Models
{
    public class RootWhatsapp{
        public string Pk {get;set;}
        public string Date {get;set;}
        public string From {get;set;}
        public string MsgContent {get;set;}
        public string MsgStatus {get;set;}
        public string MediaType {get;set;}
        public string MediaSize {get;set;} 
    }
}