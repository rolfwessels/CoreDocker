﻿using System.Reflection;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.GraphQl.DynamicQuery;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Models.Projects;
using HotChocolate.Types;
using Serilog;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsQueryType : ObjectType<ProjectsQueryType.ProjectQuery>
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()!.DeclaringType!);
        private readonly IProjectLookup _projects;

        public ProjectsQueryType(IProjectLookup projects)
        {
            _projects = projects;
        }


        protected override void Configure(IObjectTypeDescriptor<ProjectQuery> descriptor)
        {
            var options = new GraphQlQueryOptions<Project, ProjectPagedLookupOptions>(_projects.GetPaged);
            Name = "Projects";

            descriptor.Field("byId")
                .Description("Get project by id")
                .Type<NonNullType<ProjectType>>()
                .Argument("id", arg => arg.Type<NonNullType<StringType>>().Description("id of the project"))
                .Resolve(x => _projects.GetById(x.ArgumentValue<string>("id")))
                .RequirePermission(Activity.ReadProject);

            descriptor.Field("paged")
                .Description("all projects paged")
                .AddOptions(options)
                .Type<NonNullType<PagedListGraphType<Project, ProjectType>>>()
                .Resolve(x => options.Paged(x))
                .RequirePermission(Activity.ReadProject);
        }

        public class ProjectQuery
        {
        }
    }
}

/* scaffolding [
    {
      "FileName": "DefaultQuery.cs",
      "Indexline": "using CoreDocker.Api.Components.Projects;",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "using CoreDocker.Api.Components.Projects;"
      ]
    },
    {
      "FileName": "DefaultQuery.cs",
      "Indexline": "Field<ProjectsSpecification>",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "Field<ProjectsSpecification>(\"projects\",resolve: context => Task.BuildFromHttpContext(new object()));"
      ]
    },
    {
      "FileName": "IocApi.cs",
      "Indexline": "\/*project*\/",
      "InsertAbove": true,
      "InsertInline": false,
      "Lines": [
            "/*project*\/",
            "builder.RegisterType<ProjectType>().SingleInstance();",
            "builder.RegisterType<ProjectsSpecification>().SingleInstance();",
            "builder.RegisterType<ProjectCreateUpdateType>().SingleInstance();",
            "builder.RegisterType<ProjectsMutationType>().SingleInstance();",
            "",
      ]
    }



] scaffolding */