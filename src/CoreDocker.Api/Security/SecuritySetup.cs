using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.Security
{
    public class SecuritySetup
    {
        public static void AddIndentityServer4(IServiceCollection services)
        {
           // var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "damienbodserver.pfx"), "");

            
//
//            services.AddIdentity<ApplicationUser, IdentityRole>()
//                .AddEntityFrameworkStores<ApplicationDbContext>()
//                .AddDefaultTokenProviders()
//                .AddIdentityServer();

            var guestPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim("scope", "dataEventRecords")
                .Build();

            
//
//            services.AddTransient<IEmailSender, AuthMessageSender>();
//            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddIdentityServer()
//                .AddSigningCredential(cert)
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(OpenIdConfig.GetIdentityResources())
                .AddInMemoryApiResources(OpenIdConfig.GetApiResources())
                .AddInMemoryClients(OpenIdConfig.GetClients())
//                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<IdentityWithAdditionalClaimsProfileService>();
//
//            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
//                .AddIdentityServerAuthentication(options =>
//                {
//                    options.Authority = Config.HOST_URL + "/";
//                    options.AllowedScopes = new List<string> { "dataEventRecords" };
//                    options.ApiName = "dataEventRecords";
//                    options.ApiSecret = "dataEventRecordsSecret";
//                    options.SupportedTokens = SupportedTokens.Both;
//                });

           

            services.AddAuthorization(options =>
            {
                options.AddPolicy("dataEventRecordsAdmin", policyAdmin =>
                {
                    policyAdmin.RequireClaim("role", "dataEventRecords.admin");
                });
                options.AddPolicy("admin", policyAdmin =>
                {
                    policyAdmin.RequireClaim("role", "admin");
                });
                options.AddPolicy("dataEventRecordsUser", policyUser =>
                {
                    policyUser.RequireClaim("role", "dataEventRecords.user");
                });

            });

        }

        public static void Add(ContainerBuilder builder)
        {
            builder.RegisterType<IdentityWithAdditionalClaimsProfileService>().As<IProfileService>();
        }
    }
}

//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

//IdentityServerAuthenticationOptions identityServerValidationOptions = new IdentityServerAuthenticationOptions
//{
//    Authority = Config.HOST_URL + "/",
//    AllowedScopes = new List<string> { "dataEventRecords" },
//    ApiSecret = "dataEventRecordsSecret",
//    ApiName = "dataEventRecords",
//    AutomaticAuthenticate = true,
//    SupportedTokens = SupportedTokens.Both,
//    // TokenRetriever = _tokenRetriever,
//    // required if you want to return a 403 and not a 401 for forbidden responses
//    AutomaticChallenge = true,
//};

//app.UseIdentityServerAuthentication(identityServerValidationOptions);