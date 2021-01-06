using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading;
using WebNode.Controllers;

namespace WebNode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Thread(() => {
                ApiController.Run();
            }).Start();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                   .UseKestrel(o =>
                   {
                       o.Listen(IPAddress.Any, Convert.ToInt32(GlobalVars.listenPort));
                       // o.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(3);
                   })
                   .UseUrls($"http://0.0.0.0:{GlobalVars.listenPort}")
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .ConfigureAppConfiguration((hostingContext, config) =>
                   {
                       config
                           .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                           .AddJsonFile("appsettings.json", true, true)
                           .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                           .AddEnvironmentVariables();
                   })
                   .ConfigureLogging((hostingContext, logging) =>
                   {
                       logging.ClearProviders();
                       logging.AddConsole();
                   })
                   .UseStartup<Startup>();
                });
    }
}
