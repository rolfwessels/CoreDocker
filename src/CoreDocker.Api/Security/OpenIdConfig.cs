using System;
using System.Collections.Generic;
using CoreDocker.Utilities.Helpers;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace CoreDocker.Api.Security
{
    public class OpenIdConfig
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

        public static IEnumerable<ApiResource> GetApiResources(OpenIdSettings openIdSettings)
        {
            return new List<ApiResource>
            {
                new ApiResource(openIdSettings.ApiResourceName)
                {
                    ApiSecrets =
                    {
                        new Secret(openIdSettings.ApiResourceSecret.Sha256())
                    },
                    Scopes =
                    {
                        new Scope
                        {
                            Name = openIdSettings.ScopeApi,
                            DisplayName = "Standard api access"
                        }
                    },
                    UserClaims =
                    {
                        JwtClaimTypes.Role,
                        JwtClaimTypes.GivenName,
                        IdentityServerConstants.StandardScopes.Email,
                        JwtClaimTypes.Id,
                        JwtClaimTypes.Name
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients(OpenIdSettings openIdSettings)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "CoreDocker Api",
                    ClientId = openIdSettings.ClientName,
                    RequireConsent = false,
                    AccessTokenType = AccessTokenType.Reference,
                    AccessTokenLifetime = (int) TimeSpan.FromDays(1).TotalSeconds, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret(openIdSettings.ClientSecret.Sha256())
                    },
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        openIdSettings.HostUrl
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        openIdSettings.HostUrl.UriCombine("/Unauthorized")
                    },
                    AllowedCorsOrigins = openIdSettings.GetOriginList(),
                    AllowedScopes = new List<string>
                    {
                        openIdSettings.ScopeApi
                    }
                }
            };
        }
    }
}