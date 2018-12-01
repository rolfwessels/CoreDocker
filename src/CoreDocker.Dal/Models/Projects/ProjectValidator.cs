using System.Diagnostics.CodeAnalysis;
using CoreDocker.Dal.Validation;
using FluentValidation;

namespace CoreDocker.Dal.Models.Projects
{
    public class ProjectValidator : AbstractValidator<Project>
    {
        public ProjectValidator()
        {
            RuleFor(x => x.Name).NotNull().MediumString();
        }
    }
}

/* scaffolding [
    {
      "FileName": "IocCoreBase.cs",
      "Indexline": ".As<IValidator<Project>>();",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "builder.RegisterType<ProjectValidator>().As<IValidator<Project>>();"
      ]
    }
] scaffolding */