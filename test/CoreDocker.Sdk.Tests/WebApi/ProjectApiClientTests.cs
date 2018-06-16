using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Dal.Models;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Sdk.Tests.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Reference;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Utilities.Tests.TempBuildres;
using FizzWare.NBuilder;
using FluentAssertions;
using GraphQL;
using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Types;
using NUnit.Framework;

namespace CoreDocker.Sdk.Tests.WebApi
{
    [TestFixture]
    [Category("Integration")]
    public class
        ProjectApiClientTests : CrudComponentTestsBase<ProjectModel, ProjectCreateUpdateModel, ProjectReferenceModel>
    {
        private ProjectApiClient _projectApiClient;

        #region Setup/Teardown

        protected override void Setup()
        {
            var connection = _adminConnection.Value;
            _projectApiClient = connection.Projects;
            SetRequiredData(_projectApiClient);
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        [Test]
        public async Task Graph_Given_Shouldresult()
        {
            // arrange
            Setup();
            var heroRequest = new GraphQLRequest
            {
                Query = @"
                {
                    projects {
                        recent(first: 4) {
                            id
                        }
                    }
                }
            "};
            // action
            var graphQlResponse = await _adminConnection.Value.GraphQlPost(heroRequest);
            List<ProjectModel> personType = CastHelper.DynamicCastTo<List<ProjectModel>>(graphQlResponse.Data.projects.recent);
            // assert
            personType.Count.Should().BeGreaterOrEqualTo(1).And.BeLessOrEqualTo(4);
            personType.Should().OnlyContain(x => x.Name == null);
            personType.Should().OnlyContain(x => x.Id != null);
        }
     
        protected override IList<ProjectCreateUpdateModel> GetExampleData()
        {
            return Builder<Project>.CreateListOfSize(2).WithValidData().Build()
                .DynamicCastTo<List<ProjectCreateUpdateModel>>();
        }
    }

    
}