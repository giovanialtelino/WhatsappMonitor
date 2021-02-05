using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WhatsappMonitor.API.Context;

namespace WhatsappMonitor.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                })
                .ConfigureAppConfiguration(conf =>
                {
                    conf
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile("appsettings.Development.json", true)
                    .AddEnvironmentVariables();
                });
    }
}