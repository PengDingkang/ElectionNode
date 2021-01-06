using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebNode.Controllers;

namespace WebNode
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            ApiController.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                   .UseKestrel(o =>
                   {
                       o.Listen(IPAddress.Loopback, Convert.ToInt32(GlobalVars.listenPort));
                       o.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(3);
                   })
                   .UseUrls($"http://localhost:{GlobalVars.listenPort}")
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
