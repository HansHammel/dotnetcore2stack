using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace IS4ProtectedAPI
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private readonly CancellationTokenSource _consulCancellationSource = new CancellationTokenSource();
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
            .AddOptions()
            .Configure<ServiceDisvoveryOptions>(Configuration.GetSection("ServiceDiscovery"))
            .AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
                {
                    var _serviceConfiguration = p.GetRequiredService<IOptions<ServiceDisvoveryOptions>>().Value;

                    if (!string.IsNullOrEmpty(_serviceConfiguration.Consul.HttpEndpoint))
                    {
                        // if not configured, the client will use the default value "127.0.0.1:8500"
                        cfg.Address = new Uri(_serviceConfiguration.Consul.HttpEndpoint);
                    }
                }))
            .AddMvcCore()
            .AddApiExplorer() //needed by swagger
                // Note: SwaggerGen won't find controllers that are routed by convention (vs. attributes)!!!
            .AddAuthorization()
            .AddJsonFormatters();
             
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "api1";
                });

            //services.AddMvc(); //removed in favoure of .AddMvcCore()
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "My API - V1",
                        Version = "v1",
                        Description = "A sample API to demo Swashbuckle",
                        TermsOfService = "Knock yourself out",
                        Contact = new Contact
                        {
                            Name = "Joe Developer",
                            Email = "joe.developer@tempuri.org"
                        },
                        License = new License
                        {
                            Name = "Apache 2.0",
                            Url = "http://www.apache.org/licenses/LICENSE-2.0.html"
                        }
                    }
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory, 
            IApplicationLifetime appLifetime,
            IOptions<ServiceDisvoveryOptions> serviceOptions,
            IConsulClient consul)

        {
            loggerFactory
                .AddConsole(Microsoft.Extensions.Logging.LogLevel.Debug)
                .AddDebug(Microsoft.Extensions.Logging.LogLevel.Debug);

            // another approach doesn't work with wildcards like UseUrls("*:5000)) 
            /*
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.HttpPort}";

                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(address, "HealthCheck").OriginalString
                };

                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    Address = address.Host,
                    ID = serviceId,
                    Name = serviceOptions.Value.ServiceName,
                    HttpPort = address.HttpPort
                };

                consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                appLifetime.ApplicationStopping.Register(() =>
                {
                    consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                });
            }

            */
            // end another approach

            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopping.Register(_consulCancellationSource.Cancel);
            appLifetime.ApplicationStopped.Register(OnStopped);

            /*
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                appLifetime.StopApplication();
                // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };
            */

            if (env.IsDevelopment())
            {
                // Development configuration
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Staging/Production configuration
                // app.UseExceptionHandler("/error");
            }

            // IS4
            app.UseAuthentication();

            app.UseMvc();

            //needed by swagger
            app.UseStaticFiles();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            //goto http://localhost:5001/identity -> 401 requires authentification
        }

        private bool started = false;
        private void OnStarted()
        {
            started = true;
            // Perform post-startup activities here
            // TODO: use the singleton alternatively use https://github.com/wintoncode/Winton.Extensions.Configuration.Consul
            using (var client = new ConsulClient()) // uses default host:port which is localhost:8500
            {
                var tcpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    TCP = Program.serviceConfiguration.Host+":"+ Program.serviceConfiguration.HttpPort
                };
                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60),
                    Interval = TimeSpan.FromSeconds(10),
                    HTTP = $@"{Program.serviceConfiguration.URL}/HealthCheck",
                    Timeout = TimeSpan.FromSeconds(5)
                };
                var agentReg = new AgentServiceRegistration()
                {
                    Address = Program.serviceConfiguration.Host,
                    // BUG: this is horrorly wrong should contain the id of the instance but deregistration not working at the moment
                    ID = Program.serviceConfiguration.ServiceID,
                    Name = Program.serviceConfiguration.Name,
                    Checks = new AgentServiceCheck[] { tcpCheck, httpCheck },
                    Port = Program.freePort
                };
                // TODO: catch registration fails and shutdown or retry
                var r = client.Agent.ServiceRegister(agentReg).GetAwaiter().GetResult(); //throws http errors
                if (r.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Service registred with consul");
                }
                else throw new ServiceException("Service registration failed"); //don't swallow a faild registration

                //test 
                var consulResult = client.Catalog.Service(Program.serviceConfiguration.Name).GetAwaiter().GetResult();
                var healthResult = client.Health.Service(Program.serviceConfiguration.Name, tag: null, passingOnly: true);
            }
        }

        private void OnStopping()
        {
            //check if started successfully at least once
            if (started)
            {
                // Perform on-stopping activities here
                Console.WriteLine("API Server shutting down...");
                using (var client = new ConsulClient())
                {
                    var id = Program.serviceConfiguration.ServiceID;
                    // BUG: this is horrorly wrong should be the id of the instance not the name of the service
                    var r = client.Agent.ServiceDeregister(Program.serviceConfiguration.ServiceID).GetAwaiter().GetResult();
                    if (r.StatusCode == System.Net.HttpStatusCode.OK)
                        Console.WriteLine("Service deregistred");
                    else throw new ServiceException("Service deregistration failed"); //don't swallow a faild registration
                                                                                      //client.Dispose();
                }
            }
        }

        private void OnStopped()
        {
            // Perform post-stopped activities here
            Console.WriteLine("API Server was shut down. Press a key to exit.");
        }
    }
}
