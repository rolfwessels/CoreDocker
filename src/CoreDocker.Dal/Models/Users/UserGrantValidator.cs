using CoreDocker.Dal.Validation;
using FluentValidation;

namespace CoreDocker.Dal.Models.Users
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    public class UserGrantValidator : AbstractValidator<UserGrant>
    {
        public UserGrantValidator()
        {
            RuleFor(x => x.Key).NotNull();
        }
    }
}