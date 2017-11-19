using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
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
        public class ServiceConfiguartion
        {
            private string _transport;
            private int _port;
            private string _host;

            public int Port { get; }
            public string Transport { get; }
            public string Host { get; }
            public string URL { get { return _transport + "://" + _host + ":" + _port;  } }
            public string ListeningURL { get { return _transport + "://*:" + _port; } }
            public string ServiceID { get { return System.Guid.NewGuid().ToString(); }  }
            public string Name { get {
                    var a = Assembly.GetEntryAssembly();
                    return a.GetName().Name + " v" + a.GetName().Version;
                }
            }

            public ServiceConfiguartion(int port): this("http", "localhost", port) { }
            public ServiceConfiguartion(string transport = "http", string host = "localhost", int port = 5000)
            {
                Port = _port = port;
                Host = _host = host;
                Transport = _transport = transport;
            }
        }

        public static ServiceConfiguartion serviceConfiguration;
        public static int freePort = 5000;

        public static void Main(string[] args)
        {
            Boolean retry = true;
            IWebHost host;
            while (retry)
            {
                try
                {
                    serviceConfiguration = new ServiceConfiguartion(freePort);
                    host = BuildWebHost(args);
                    host.Run();
                    retry = false;
                }
                catch (System.IO.IOException ex)
                {
                    // try another port
                    if (ex.InnerException.Message.Contains("EADDRINUSE address already in use"))
                    {
                        freePort += 1;
                    }
                    else throw; // rethrow
                } finally
                {
                    host = null;
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
                .UseUrls(serviceConfiguration.ListeningURL)
                .UseStartup<Startup>()
                .Build();
    }
}
