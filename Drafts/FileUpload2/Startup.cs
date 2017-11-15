using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FileUpload2.Middleware;

// TODO: maybe obsolete soon - Microsoft.Extensions.PlatformAbstractions http://michaco.net/blog/EnvironmentVariablesAndConfigurationInASPNETCoreApps
/*
 * //some thoughts about paths:
var basePath = PlatformServices.Default.Application.ApplicationBasePath;
// the app's name and version
var appName = PlatformServices.Default.Application.ApplicationName;
var appVersion = PlatformServices.Default.Application.ApplicationVersion;
// object with some dotnet runtime version information
var runtimeFramework = PlatformServices.Default.Application.RuntimeFramework;
System.AppContext.BaseDirectory
PlatformServices.Default.Application.ApplicationBasePath
AppDomain.CurrentDomain.BaseDirectory
*/

namespace FileUpload2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //private IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot");
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(path));
            services.AddMvc();
        }

  

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            string webRootPath = env.WebRootPath;
            string contentRootPath = env.ContentRootPath;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                /*
                app.UseExceptionHandler(configure =>
                {
                    configure.Run(async context =>
                    {
                        var ex = context.Features
                                        .Get<IExceptionHandlerFeature>()
                                        .Error;

                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync($"{ex.Message}");
                    });
                });
                */
            }
            //app.UseMiddleware<FileUploadProviderMiddleware>();
            app.UseMvc();
        }
    }
}
