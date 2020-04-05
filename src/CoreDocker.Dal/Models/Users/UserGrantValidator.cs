using System.Diagnostics.CodeAnalysis;
using CoreDocker.Dal.Validation;
using FluentValidation;

namespace CoreDocker.Dal.Models.Users
{
    public class UserGrantValidator : AbstractValidator<UserGrant>
    {
        public UserGrantValidator()
        {
            RuleFor(x => x.Key).NotNull().MediumString();
            RuleFor(x => x.User).NotNull()
                .Must(x => !string.IsNullOrEmpty(x?.Name))
                .Must(x => !string.IsNullOrEmpty(x?.Email))
                .WithMessage("User reference not set or complete");
        }
    }
}