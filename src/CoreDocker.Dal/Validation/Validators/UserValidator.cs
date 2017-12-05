﻿using CoreDocker.Dal.Models;
using FluentValidation;

namespace CoreDocker.Dal.Validation.Validators
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
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