using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IS4ProtectedAPI
{
    public partial class Program
    {

        private static CancellationTokenSource cancelTokenSource = new System.Threading.CancellationTokenSource();
        public static void Shutdown()
        {
            cancelTokenSource.Cancel();
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
                    //host.RunAsync(cancelTokenSource.Token);
                    host.Run();
                    retry = false;
                }
                catch (System.IO.IOException ex)
                {
                    // try another port
                    if (ex.InnerException.Message.Contains("EADDRINUSE address already in use"))
                    {
                        freePort += 1;
                        Shutdown();
                    }
                    else
                    {
                        Shutdown();
                        throw; // rethrow
                    }
                }
            }
            Console.ReadKey();
        }


        public static IWebHost BuildWebHost(string[] args)
        {
            return new WebHostBuilder()
                //     Defaults to half of System.Environment.ProcessorCount rounded down and clamped
                //     between 1 and 16.
                .UseLibuv(opts => opts.ThreadCount = 1)
                .UseKestrel()
                /*
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, Program.serviceConfiguration.HttpsPort, listenOptions =>
                    {
                        listenOptions.UseHttps("certificate.pfx", "password");
                    });
                })
                */
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    if (env.IsDevelopment())
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        if (appAssembly != null)
                        {
                            config.AddUserSecrets(appAssembly, optional: true);
                        }
                    }

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                // -- custom start --
                .CaptureStartupErrors(true)
                // bind to all nics using specific port
                .UseUrls(serviceConfiguration.ListeningURL)
                // -- custom end --
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .UseStartup<Startup>()
                .Build();
        }

        /*
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseConfiguration(config)
                //.UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    IHostingEnvironment env = builderContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                //.UseIISIntegration()
                .CaptureStartupErrors(true)
                // bind to all nics using specific port
                .UseUrls(serviceConfiguration.ListeningURL)
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .UseStartup<Startup>()
                .Build();
        */
    }
}
