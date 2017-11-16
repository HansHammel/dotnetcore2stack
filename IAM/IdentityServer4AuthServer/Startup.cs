using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4AuthServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // add IdentityServer service to DI
            // AddDeveloperSigningCredential - extension creates temporary key material for signing tokens for development
            // services.AddIdentityServer().AddDeveloperSigningCredential();

            // client credentials flow -> ClientCredentialsFlowConfig.cs
            // configure identity server with in-memory stores, keys, clients and resources
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(ClientCredentialsFlowConfig.GetApiResources())
                .AddInMemoryClients(ClientCredentialsFlowConfig.GetClients());
            // navigate to http://localhost:5000/.well-known/openid-configuration
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

            // add IdentityServer middleware to HTTP pipeline
            // use an inmemory store for development
            // for Production use EF Integration
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

            app.UseIdentityServer();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

        private void OnStarted()
        {
            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            // Perform on-stopping activities here
            Console.WriteLine("Auth Server shutting down...");
        }

        private void OnStopped()
        {
            // Perform post-stopped activities here
            Console.WriteLine("Auth Server was shut down. Press a key to exit.");
            Console.ReadKey();
        }
    }

}
