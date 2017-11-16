
- Implicit - optimized for browser-based applications. Either for user authentication-only (both server-side and JavaScript applications), or authentication and access token requests (JavaScript applications). In the implicit flow, all tokens are transmitted via the browser, and advanced features like refresh tokens are thus not allowed.
- Authorization code - provides a way to retrieve tokens on a back-channel as opposed to the browser front-channel. it is generally recommended you combine that with identity tokens which turns it into the so called hybrid flow
- Hybrid - is a combination of the implicit and authorization code flow - it uses combinations of multiple grant types. recommended flow for native applications that want to retrieve access tokens (and possibly refresh tokens as well) and is used for server-side web applications and native desktop/mobile applications
- Client credentials - for server to server communication 
- Resource owner password - none interactive, recommendation is to use an interactive flow like implicit or hybrid for user authentication instead.
- Refresh tokens - gaining long lived access to APIs
- Extension grants - allow extending the token endpoint with new grant types


The most common authentication protocols are SAML2p, WS-Federation and OpenID Connect – SAML2p being the most popular and the most widely deployed.
OpenID Connect is the newest of the three, but is considered to be the future because it has the most potential for modern applications
OpenID Connect and OAuth 2.0 are very similar – in fact OpenID Connect is an extension on top of OAuth 2.0.

Client - a user (requesting an identity token) or for accessing a resource (requesting an access token). 
Resource - has a unique name , something you want to protect with IdentityServer - either identity data of your users, or APIs.
Identity data Identity information (aka claims) about a user, e.g. name or email address.
Identity Token - the outcome of an authentication process. It contains at a bare minimum an identifier for the user (called the sub aka subject claim) 
Access Token - allows access to an API resource


OpenID Connect
•OpenID Connect Core 1.0 (spec)
•OpenID Connect Discovery 1.0 (spec)
•OpenID Connect Session Management 1.0 - draft 28 (spec)
•OpenID Connect Front-Channel Logout 1.0 - draft 02 (spec)
•OpenID Connect Back-Channel Logout 1.0 - draft 04 (spec)

OAuth 2.0
•OAuth 2.0 (RFC 6749)
•OAuth 2.0 Bearer Token Usage (RFC 6750)
•OAuth 2.0 Multiple Response Types (spec)
•OAuth 2.0 Form Post Response Mode (spec)
•OAuth 2.0 Token Revocation (RFC 7009)
•OAuth 2.0 Token Introspection (RFC 7662)
•Proof Key for Code Exchange (RFC 7636)
•JSON Web Tokens for Client Authentication (RFC 7523)

IdentityServer authentication handler, for validating tokens in APIs, supporting both JWT and reference tokens in the same API
Install-Package IdentityServer4.AccessTokenValidation
dotnet add package IdentityServer4.AccessTokenValidation 

ASP.NET Core Identity provides a simple configuration API to use the ASP.NET Identity managament library 
Install-Package IdentityServer4.AspNetIdentity 
dotnet add package IdentityServer4.AspNetIdentity

EntityFramework Core provides an EntityFramework implementation for the configuration and operational stores in IdentityServer
Install-Package IdentityServer4.EntityFramework 
dotnet add package IdentityServer4.EntityFramework 

Community Help:
https://stackoverflow.com/questions/tagged/?tagnames=identityserver4&sort=newest
https://gitter.im/IdentityServer/IdentityServer4

Admin UI, Identity Express and SAML2p support (commercial)

##Setup

Create new empty ASP.Net Core Application
Install-Package IdentityServer4

Startup.cs:

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // add IdentityServer service to DI
            services.AddIdentityServer().AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // add IdentityServer middleware to HTTP pipeline
            // use an inmemory store for development
            // AddDeveloperSigningCredential extension creates temporary key material for signing tokens
            // for Production use EF Integration
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }  
	
            app.UseIdentityServer();
        }
    }


Run in console host to see the logging output (not on IIS)!
give a static IP http://localhost:5000/
configure the same port for IIS Express and self-hosting
Ctrl+F5 for running without debugging



Other JWT Options
    Install-Package IdentityServer4.AccessTokenValidation - IdentityServer authentication handler, for validating tokens in APIs, supporting both JWT and reference tokens in the same API
    dotnet add package IdentityServer4.AccessTokenValidation 
    Install-Package Microsoft.AspNetCore.Authentication.JwtBearer (used by IdentityServer authentication handler internally)
    Install-Package Microsoft.Owin.Security.Jwt 
    dotnet add package Microsoft.Owin.Security.Jwt 
    Install-Package Nancy.Authentication.JwtBearer 
    dotnet add package Nancy.Authentication.JwtBearer 
    Install-Package System.IdentityModel.Tokens.Jwt
    dotnet add package System.IdentityModel.Tokens.Jwt 
    Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory
    dotnet add package Microsoft.IdentityModel.Clients.ActiveDirectory 