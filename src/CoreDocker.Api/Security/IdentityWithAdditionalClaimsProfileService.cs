using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreDocker.Core;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models;
using CoreDocker.Utilities.Helpers;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;

namespace CoreDocker.Api.Security
{
    public class IdentityWithAdditionalClaimsProfileService : IProfileService , IResourceOwnerPasswordValidator
    {
        private readonly IRoleManager _roleManager;
        private readonly IUserManager _userManager;


        public IdentityWithAdditionalClaimsProfileService(IUserManager userManager, IRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region IProfileService Members

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();

            var user = await _userManager.GetUserByEmail(sub);
            
            var claims = BuildClaimListForUser(user);

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.GetUserByEmail(sub);
            context.IsActive = user != null;
        }

        #endregion


        #region Implementation of IResourceOwnerPasswordValidator

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userManager.GetUserByEmailAndPassword(context.UserName, context.Password);
            if (user != null)
            {
                var claims = BuildClaimListForUser(user);
                context.Result = new GrantValidationResult(
                    subject: user.Id,
                    authenticationMethod: "password",
                    claims: claims
                );
            }
        }

        #endregion

        #region Private Methods

        private static List<Claim> BuildClaimListForUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.GivenName, user.Name),
                new Claim(IdentityServerConstants.StandardScopes.Email, user.Email),
                new Claim(JwtClaimTypes.Scope, OpenIdConfigBase.ScopeApi)
            };


            claims.Add(user.Roles.Contains(RoleManager.Admin.Name)
                ? new Claim(JwtClaimTypes.Role, "admin")
                : new Claim(JwtClaimTypes.Role, "guest"));
            return claims;
        }

        #endregion
    }
}