using FluentValidation;
using CoreDocker.Dal.Models;

namespace CoreDocker.Dal.Validation
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
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