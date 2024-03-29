﻿using CoreDocker.Api.Security;
using CoreDocker.Dal.Models.Auth;
using Microsoft.AspNetCore.Authorization;

namespace CoreDocker.Api.WebApi.Attributes
{
    public class AuthorizeActivityAttribute : AuthorizeAttribute
    {
        public AuthorizeActivityAttribute()
        {
        }

        public AuthorizeActivityAttribute(Activity activities) : base(UserClaimProvider.ToPolicyName(activities))
        {
            Activities = activities;
        }

        public Activity Activities { get; }

//		protected override bool IsAuthorized(HttpActionContext actionContext)
//		{
//			var isAuthorized = base.IsAuthorized(actionContext);
//			if (isAuthorized)
//			{
//				var identity = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
//				if (identity == null)
//				{
//					_log.Error("User not authorized because we were expecting a ClaimsIdentity");
//					return false;
//				}
//			    var roleName = identity.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();
//			    isAuthorized = RoleManager.IsAuthorizedActivity(Activities, roleName);
//			}
//			return isAuthorized;
//		}
    }
}