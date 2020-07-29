using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WhatsappMonitor.API.Context;

namespace WhatsappMonitor.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MyDbContext ctx = new MyDbContext();
            StartDbContext.StartDb(ctx);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseStartup<Startup>()
                    .UseKestrel(o =>
                    o.Limits.MaxRequestBodySize = 209715200);
                });
    }
}
