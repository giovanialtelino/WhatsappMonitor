using System;
using WhatsappMonitor.Shared.Models;
using System.IO;
using System.IO.Enumeration;
using System.Threading.Tasks;

namespace WhatsappMonitor.API.Context
{
    public static class StartDbContext
    {
        public static void StartDb(MyDbContext ctx)
        {
            ctx.Database.EnsureCreated();
        }
    }
}