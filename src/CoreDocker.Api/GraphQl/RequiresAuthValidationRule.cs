using System.Security.Claims;
using GraphQL.Language.AST;
using GraphQL.Validation;
using IdentityServer4.Extensions;

namespace CoreDocker.Api.GraphQl
{
    public class RequiresAuthValidationRule : IValidationRule
    {
        #region IValidationRule Members

        public INodeVisitor Validate(ValidationContext context)
        {
            var task = GraphQlUserContext.ReadFromContext(context);

            var claimsPrincipal = task.User ?? new ClaimsPrincipal();
            var authenticated = claimsPrincipal?.IsAuthenticated() ?? false;

            return new EnterLeaveListener(_ =>
            {
                _.Match<Field>(fieldAst =>
                {
                    var fieldDef = context.TypeInfo.GetFieldDef();
                    if (fieldDef.IsAuthorizationRequire() && !authenticated)
                        context.ReportError(new ValidationError(
                            context.OriginalQuery,
                            "auth-required",
                            "Authentication required.",
                            fieldAst));
                    else if (fieldDef.RequiresPermissions() &&
                             (!authenticated || !fieldDef.CanAccess(claimsPrincipal.Claims)))
                        context.ReportError(new ValidationError(
                            context.OriginalQuery,
                            "auth-required",
                            "You are not authorized to run this query.",
                            fieldAst));
                });
            });
        }

        #endregion
    }
}