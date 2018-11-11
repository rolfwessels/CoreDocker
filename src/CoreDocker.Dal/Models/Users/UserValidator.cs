using System.Diagnostics.CodeAnalysis;
using CoreDocker.Dal.Validation;
using FluentValidation;

namespace CoreDocker.Dal.Models.Users
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name).NotNull().MediumString();
            RuleFor(x => x.Email).NotNull().EmailAddress();
            RuleFor(x => x.HashedPassword).NotEmpty();
            RuleFor(x => x.Roles).NotEmpty();
        }
    }
}