using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Sdk.Tests.Shared;
using CoreDocker.Shared.Models.Projects;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Utilities.Tests.TempBuildres;
using FizzWare.NBuilder;
using FluentAssertions;
using GraphQL.Common.Request;
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
        public async Task GraphQl_Given_Shouldresult()
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

        [Test]
        public async Task GraphQl_QueryWithCountExt_Shouldresult()
        {
            // arrange
            Setup();
            var heroRequest = new GraphQLRequest
            {
                Query = @"
                {
                    projects {
                        query(first: 4) {
                            count
                        }
                    }
                }
            "};
            // action
            var graphQlResponse = await _adminConnection.Value.GraphQlPost(heroRequest);
            object data = graphQlResponse.Data;
            data.Dump("graphQlResponse.Data");


            // assert
            int count = graphQlResponse.Data.projects.query.count;
            count.Should().BeGreaterThan(1);

        }

        protected override IList<ProjectCreateUpdateModel> GetExampleData()
        {
            return Builder<Project>.CreateListOfSize(2).WithValidData().Build()
                .DynamicCastTo<List<ProjectCreateUpdateModel>>();
        }
    }

    
}