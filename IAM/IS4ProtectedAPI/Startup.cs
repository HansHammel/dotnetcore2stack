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
                var c = new AgentServiceCheck();
                c.TCP = "http://localhost:" + Program.freePort;
                c.Interval = TimeSpan.FromSeconds(60);
                var agentReg = new AgentServiceRegistration()
                {
                    Address = Program.serviceConfiguration.Host,
                    //ID = Program.serviceConfiguration.ServiceID,
                    Name = Program.serviceConfiguration.Name,
                    Check = c,
                    Port = Program.freePort
                };
                if (client.Agent.ServiceRegister(agentReg).GetAwaiter().GetResult().StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Service registred with consul");
                }
            }
        }

        private void OnStopping()
        {
            // Perform on-stopping activities here
            Console.WriteLine("API Server shutting down...");
            using (var client = new ConsulClient())
            {
                var id = Program.serviceConfiguration.ServiceID;
                var r = client.Agent.ServiceDeregister(Program.serviceConfiguration.Name).GetAwaiter().GetResult();
                if (r.StatusCode == System.Net.HttpStatusCode.OK)
                    Console.WriteLine("Service deregistred");
            }
        }

        private void OnStopped()
        {
            // Perform post-stopped activities here
            Console.WriteLine("API Server was shut down. Press a key to exit.");
        }
    }
}
