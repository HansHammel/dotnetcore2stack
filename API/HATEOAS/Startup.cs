using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RefArc.Api.HATEOAS.OtherLayers;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;
using RefArc.Api.HATEOAS.Filter;
using Microsoft.AspNetCore.Authorization;

namespace RefArc.Api.HATEOAS
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }
/*
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
*/

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets("64f721de-f832-4bb2-8b9f-777b935813af");
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }


        public void ConfigureServices(
            IServiceCollection services)
        {
            #region Authentication
            var tokenProvider = new RsaJwtTokenProvider("issuer", "audience", "mykeyname");
            services.AddSingleton<ITokenProvider>(tokenProvider);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => {
                    /*
                                    // my API name as defined in Config.cs - new ApiResource... or in DB ApiResources table
               options.Audience = "API Name";
               // URL of Auth server(API and Auth are separate projects/applications),
               // so for local testing this is http://localhost:5000 if you followed ID4 tutorials
               options.Authority = "http://localhost:30940/";
                     */
                    options.RequireHttpsMetadata = false; //set true in production for https
                    options.SaveToken = true;
                    options.TokenValidationParameters = tokenProvider.GetValidationParameters();
                });

            // This is for the [Authorize] attributes.
            services.AddAuthorization(auth => {
                auth.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
            #endregion

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                           .ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddSingleton<IMovieService, MovieService>();

            services.AddMvc(options =>
            {
                options.ReturnHttpNotAcceptable = true;

                options.InputFormatters
                       .OfType<JsonInputFormatter>()
                       .FirstOrDefault()
                       ?.SupportedMediaTypes.Add("application/vnd.fiver.movie.input+json");

                options.OutputFormatters
                       .OfType<JsonOutputFormatter>()
                       .FirstOrDefault()
                       ?.SupportedMediaTypes.Add("application/vnd.fiver.hateoas+json");
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
            });
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env)
        {
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

            #region Authentication
            app.UseAuthentication();
            #endregion

            app.UseMvcWithDefaultRoute();

            // Set password with the Secret Manager tool.
            // dotnet user-secrets set SeedUserPW <pw>
            var testUserPw = Configuration["SeedUserPW"];

            if (String.IsNullOrEmpty(testUserPw))
            {
                throw new System.Exception("Use secrets manager to set SeedUserPW \n" +
                                           "dotnet user-secrets set SeedUserPW <pw>");
            }

            /*
            try
            {
                SeedData.Initialize(app.ApplicationServices, testUserPw).Wait();
            }
            catch
            {
                throw new System.Exception("You need to update the DB "
                    + "\nPM > Update-Database " + "\n or \n" +
                      "> dotnet ef database update"
                      + "\nIf that doesn't work, comment out SeedData and "
                      + "register a new user");
            }
            */

        }
    }
}
