using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IS4ProtectedAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
            .AddAuthorization()
            .AddJsonFormatters();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "api1";
                });

            //services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                appLifetime.StopApplication();
                // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };

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

            //goto http://localhost:5001/identity -> 401 requires authentification
        }

        private void OnStarted()
        {
            // Perform post-startup activities here
            using (var client = new ConsulClient()) // uses default host:port which is localhost:8500
            {
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
                    Checks = new AgentServiceCheck[] { httpCheck },
                    Port = Program.freePort
                };
                // TODO: catch registration fails and shutdown or retry
                var r = client.Agent.ServiceRegister(agentReg).GetAwaiter().GetResult(); //throws http errors
                if (r.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Service registred with consul");
                }
                else throw new ServiceException("Service registration failed"); //don't swallow a faild registration
            }
        }

        private void OnStopping()
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
                client.Dispose();
            }
        }

        private void OnStopped()
        {
            // Perform post-stopped activities here
            Console.WriteLine("API Server was shut down. Press a key to exit.");
        }
    }
}
