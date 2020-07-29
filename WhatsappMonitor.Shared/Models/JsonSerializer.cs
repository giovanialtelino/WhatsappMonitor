using System;

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

//{"Pk":"203340","Date":"2018-04-24 12:36:52","From":"me","MsgContent":"N/A","MsgStatus":"6","MediaType":"0","MediaSize":"19"//}