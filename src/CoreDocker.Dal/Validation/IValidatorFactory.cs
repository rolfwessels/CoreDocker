using FluentValidation;
using FluentValidation.Results;
using CoreDocker.Dal.Models;

namespace CoreDocker.Dal.Validation
{
    public interface IValidatorFactory
    {
        ValidationResult For<T>(T user);
        void ValidateAndThrow<T>(T user);
        IValidator<T> Validator<T>();
    }

    
}