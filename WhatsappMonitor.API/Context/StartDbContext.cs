using System;
using WhatsappMonitor.Shared.Models;
using System.IO;
using System.IO.Enumeration;
using System.Threading.Tasks;

namespace WhatsappMonitor.API.Context
{
    public class StartDbContext
    {
        public void StartDb()
        {
            string dbName = "Whatsapp.db";
            if (!File.Exists(dbName))
            {
                File.Create(dbName);
            }

            using (var dbContext = new MyDbContext())
            {
                dbContext.Database.EnsureCreated();
            }
        }
    }
}