﻿using Bumbershoot.Utilities.Helpers;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace CoreDocker.Api.Security
{
    public class OpenIdConfig
    {
        public const string Scope = "api";

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
                new(openIdSettings.ApiResourceName)
                {
                    ApiSecrets =
                    {
                        new Secret(openIdSettings.ApiResourceSecret.Sha256())
                    },
                    Scopes = GetApiScopes(openIdSettings).Select(x => x.Name).ToArray(),
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

        public static IEnumerable<ApiScope> GetApiScopes(OpenIdSettings openIdSettings)
        {
            return new[]
            {
                new ApiScope(OpenIdConfig.Scope, "Standard api access")
            };
        }

        

        public static IEnumerable<Client> GetClients(OpenIdSettings openIdSettings)
        {
            return new List<Client>
            {
                new()
                {
                    ClientName = "CoreDocker Api",
                    ClientId = openIdSettings.ClientName,
                    RequireConsent = false,
                    AccessTokenType = openIdSettings.UseReferenceTokens
                        ? AccessTokenType.Reference
                        : AccessTokenType.Jwt,
                    AccessTokenLifetime = (int)TimeSpan.FromDays(1).TotalSeconds, // 10 minutes, default 60 minutes
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
                        Scope, IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Email,
                    }
                }
            };
        }
    }
}