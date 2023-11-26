using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreDocker.Api.AppStartup;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;

namespace CoreDocker.Api.Security
{
    public class UserClaimProvider : IProfileService, IResourceOwnerPasswordValidator
    {
        private readonly IRoleManager _roleManager;
        private readonly IUserLookup _userLookup;


        public UserClaimProvider(IUserLookup userLookup, IRoleManager roleManager)
        {
            _userLookup = userLookup;
            _roleManager = roleManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();

            var user = (await _userLookup.GetUserByEmail(sub)).ExistsOrThrow(sub);

            var claims = BuildClaimListForUser(user);

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userLookup.GetUserByEmail(sub);
            context.IsActive = user != null;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userLookup.GetUserByEmailAndPassword(context.UserName, context.Password);
            if (user != null)
            {
                var claims = BuildClaimListForUser(user);
                context.Result = new GrantValidationResult(
                    user.Id,
                    "password",
                    claims
                );
            }
        }

        public static string ToPolicyName(Activity claim)
        {
            return claim.ToString().ToLower();
        }

        private List<Claim> BuildClaimListForUser(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtClaimTypes.Name, user.Email ?? throw new Exception("Email required for claim")),
                new(JwtClaimTypes.Id, user.Id),
                new(JwtClaimTypes.GivenName, user.Name ?? throw new Exception("Email required for claim")),
                new(IdentityServerConstants.StandardScopes.Email, user.Email),
                new(JwtClaimTypes.Scope, IocApi.Instance.Resolve<OpenIdSettings>().ScopeApi),
                user.Roles.Contains(RoleManager.Admin.Name)
                    ? new Claim(JwtClaimTypes.Role, RoleManager.Admin.Name)
                    : new Claim(JwtClaimTypes.Role, RoleManager.Guest.Name)
            };
            var selectMany = user.Roles.Select(r => _roleManager.GetRoleByName(r).Result).SelectMany(x => x!.Activities)
                .Distinct().ToList();
            claims.AddRange(selectMany.Select(claim => new Claim(JwtClaimTypes.Role, ToPolicyName(claim))));

            return claims;
        }
    }
}