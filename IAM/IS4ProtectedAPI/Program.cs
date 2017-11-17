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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IS4ProtectedAPI
{
    public class Program
    {
        private static int? FreeTcpPort(int startPort, int endPort)
        {
            int? port = startPort;
            TcpListener l;
            if (port >= startPort && port <= endPort)
            {
                string host = "localhost";
                int port2 = startPort;
                var x = Dns.GetHostAddresses(host);
                IPAddress addr = (IPAddress)x[1];
                try
                {
                    TcpListener tcpList = new TcpListener(addr, port2);
                    tcpList.ExclusiveAddressUse = true;
                    tcpList.Start();
                    return port2;
                }
                catch (SocketException ex)
                {
                    // Catch exception here if port is blocked
                    Console.WriteLine(ex);
                    return null;
                }
                /*
                try
                {
                    l = new TcpListener(IPAddress.Loopback, startPort);
                    l.ExclusiveAddressUse = true;
                    l.Start();
                    port = ((IPEndPoint)l.LocalEndpoint).Port;
                    while (l.Server.IsBound)
                    {
                        l.Stop();
                        return port;
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                */
            }
            return null;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine(FreeTcpPort(5000, 5010));
            //Console.ReadKey();
            BuildWebHost(args).Run();
        }


        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseConfiguration(config)
                //.UseKestrel()
                //.UseContentRoot(Directory.GetCurrentDirectory())
                //.UseIISIntegration()
                .CaptureStartupErrors(true)
                // bind to all nics using specific port
                .UseUrls("http://*:5000")
                .UseStartup<Startup>()
                .Build();
    }
}
