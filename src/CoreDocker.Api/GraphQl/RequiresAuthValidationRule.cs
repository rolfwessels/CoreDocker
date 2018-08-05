using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreDocker.Utilities.Helpers;
using GraphQL.Language.AST;
using GraphQL.Validation;
using IdentityServer4.Extensions;

namespace CoreDocker.Api.GraphQl
{
    public class RequiresAuthValidationRule : IValidationRule
    {
        public INodeVisitor Validate(ValidationContext context)
        {
            var task = (Task<GraphQlSetup.GraphQLUserContext>) context.UserContext;


            var claimsPrincipal = task.Result?.User ?? new ClaimsPrincipal();
            var authenticated = claimsPrincipal?.IsAuthenticated() ?? false;

            return new EnterLeaveListener(_ =>
            {
                _.Match<Operation>(op =>
                {
                    if (op.OperationType == OperationType.Mutation && !authenticated)
                    {
                        context.ReportError(new ValidationError(
                            context.OriginalQuery,
                            "auth-required",
                            $"Authorization is required to access {op.Name}.",
                            op));
                    }
                });

                // this could leak info about hidden fields in error messages
                // it would be better to implement a filter on the schema so it
                // acts as if they just don't exist vs. an auth denied error
                // - filtering the schema is not currently supported
                _.Match<Field>(fieldAst =>
                {
                    var fieldDef = context.TypeInfo.GetFieldDef();
                    if (fieldDef.RequiresPermissions() &&
                        (!authenticated || !fieldDef.CanAccess(claimsPrincipal.Claims)))
                    {
                        context.ReportError(new ValidationError(
                            context.OriginalQuery,
                            "auth-required",
                            $"You are not authorized to run this query.",
                            fieldAst));
                    }
                });
            });
        }
    }
}