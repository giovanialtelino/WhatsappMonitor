using System;
using System.Collections.Generic;
using System.Text;

namespace WhatsappMonitor.Shared.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Group> Groups {get;set;}
    }
}