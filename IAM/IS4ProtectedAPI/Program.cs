using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IS4ProtectedAPI
{
    public class Program
    {
        private static int freePort = 5000;

        public static void Main(string[] args)
        {
            Boolean succeeded = false;
            while (!succeeded)
            {
                try
                {
                    BuildWebHost(args).Run();
                }
                catch (System.IO.IOException ex)
                {
                    // try another port
                    if (ex.InnerException.Message.Contains("EADDRINUSE address already in use"))
                    {
                        freePort += 1;
                    }
                    else throw; // rethrow
                }
            }
            Console.ReadKey();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseConfiguration(config)
                //.UseKestrel()
                //.UseContentRoot(Directory.GetCurrentDirectory())
                //.UseIISIntegration()
                .CaptureStartupErrors(true)
                // bind to all nics using specific port
                .UseUrls("http://*:" + freePort.ToString())
                .UseStartup<Startup>()
                .Build();
    }
}
