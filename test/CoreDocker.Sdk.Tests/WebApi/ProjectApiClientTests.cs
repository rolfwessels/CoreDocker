using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Sdk.RestApi;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Sdk.Tests.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Projects;
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

        [Test]
        public async Task Graph_QueryWithCountExt_Shouldresult()
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

        [Test]
        public async Task Graph_QueryMeWithLoggedInUser_ShouldReturnCurrentUserName()
        {
            // arrange
            Setup();
            var heroRequest = new GraphQLRequest
            {
                Query = @"
                {
                    users {
                        me {
                            name
                        }
                    }
                }
            "
            };
            // action
            var graphQlResponse = await _adminConnection.Value.GraphQlPost(heroRequest);
            object data = graphQlResponse.Data;
            data.Dump("graphQlResponse.Data");
            
            // assert
            string name = graphQlResponse.Data.users.me.name;
            name.Should().Be("Admin user");

        }


        [Test]
        public async Task Graph_QueryMeWithNonLoggedInUser_ShouldReturnCurrentUserName()
        {
            // arrange
            Setup();
            var heroRequest = new GraphQLRequest
            {
                Query = @"
                {
                    users {
                        me {
                            name
                        }
                    }
                }
            "
            };
            // action
            var adminConnectionValue = (CoreDockerClient) _defaultRequestFactory.Value.GetConnection();
            Action testCall = () => { adminConnectionValue.GraphQlPost(heroRequest).Wait(); };
            testCall.Should().Throw<Exception>().WithMessage("Error trying to resolve me.")
                .And.ToFirstExceptionOfException().GetType().Name.Should().Be("GraphQlResponseException");


        }

        protected override IList<ProjectCreateUpdateModel> GetExampleData()
        {
            return Builder<Project>.CreateListOfSize(2).WithValidData().Build()
                .DynamicCastTo<List<ProjectCreateUpdateModel>>();
        }
    }

    
}