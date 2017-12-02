using System;
using System.Collections.Generic;
using CoreDocker.Core;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace CoreDocker.Api.Security
{
    public class OpenIdConfig : OpenIdConfigBase
    {
       

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(ApiResourceName)
                {
                    ApiSecrets =
                    {
                        new Secret(ApiResourceSecret.Sha256())
                    },
                    Scopes =
                    {
                        new Scope
                        {
                            Name = ScopeApi,
                            DisplayName = "Standard api access"
                        }
                    },
                    UserClaims = { JwtClaimTypes.Role , JwtClaimTypes.GivenName , IdentityServerConstants.StandardScopes.Email }
                },
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "CoreDocker Api",
                    ClientId = ClientName,
                    RequireConsent = false,
                    AccessTokenType = AccessTokenType.Reference,
                    AccessTokenLifetime = (int)TimeSpan.FromDays(1).TotalSeconds, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret(ClientSecret.Sha256())
                    },
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        HostUrl
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        HostUrl + "/Unauthorized"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        HostUrl
                    },
                    AllowedScopes = new List<string>
                    {
                        ScopeApi
                    }
                }
            };
        }

       
    }
}