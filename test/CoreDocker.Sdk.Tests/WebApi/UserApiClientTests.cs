using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Sdk.RestApi;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Sdk.Tests.Shared;
using CoreDocker.Shared.Models.Users;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Utilities.Tests.TempBuildres;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentAssertions.Equivalency;
using GraphQL.Common.Request;
using log4net;
using NUnit.Framework;

namespace CoreDocker.Sdk.Tests.WebApi
{
    [TestFixture]
    [Category("Integration")]
    public class UserApiClientTests : CrudComponentTestsBase<UserModel, UserCreateUpdateModel, UserReferenceModel>
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private UserApiClient _userApiClient;

        #region Setup/Teardown

        protected override void Setup()
        {
            var connection = _adminConnection.Value;
            _userApiClient = connection.Users;
            SetRequiredData(_userApiClient);
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion


        [Test]
        public async Task GraphQl_EnsureCorrectDateFormat_ShouldMatchApiAndDb()
        {
            // arrange
            Setup();
            var request = new GraphQLRequest
            {
                Query = @"
                {
                    users {
                        me {
                            updateDate
                        }
                    }
                }
            "
            };

            // action
            var graphQlResponse = await _adminConnection.Value.GraphQlPost(request);
            var userModel = await _adminConnection.Value.Users.WhoAmI();
            object data = graphQlResponse.Data;
            data.Dump("graphQlResponse.Data");

            // assert
            DateTime updateDate = graphQlResponse.Data.users.me.updateDate;
            var userModelUpdateDate = userModel.UpdateDate;
            updateDate.ToUniversalTime().Should().Be(userModelUpdateDate.ToUniversalTime());
        }

        [Test]
        public async Task GraphQl_EnsureCrud_ShouldCreateUpdateDeleteUser()
        {
            // arrange
            Setup();
            var userCreate = GetExampleData().First();

            // action
            var insertResponst = await _adminConnection.Value.GraphQlPost(new GraphQLRequest
            {
                Query = $@"
                mutation {{
                  users {{
                    insert (user:{{name:""{userCreate.Name.Mask(10, "...")}"",email:""{userCreate.Email}"",password:""{userCreate.Password}""}}) {{
                                id
                            }}
                        }}
                    }}"
            });
            string id = insertResponst.Data.users.insert.id;
            Action testCall = () =>
            {
                _adminConnection.Value.GraphQlPost(new GraphQLRequest
                {
                    Query = $@"
                mutation {{
                  users {{
                    update (id:""{id}"", user:{{name:""{userCreate.Name}"",email:""asdf@invalid"",password:""{userCreate.Password}""}}) {{
                                id
                            }}
                        }}
                    }}"
                }).Wait();
            };
            testCall.Should().Throw<GraphQlResponseException>();

            var updateResponst = await _adminConnection.Value.GraphQlPost(new GraphQLRequest
            {
                Query = $@"
                mutation {{
                  users {{
                    update (id:""{id}"", user:{{name:""{userCreate.Name}"",email:""{userCreate.Email}"",password:""{userCreate.Password}""}}) {{
                                id
                            }}
                        }}
                    }}"
            });
            string updateId = updateResponst.Data.users.update.id;
            var deleteResponse = await _adminConnection.Value.GraphQlPost(new GraphQLRequest
            {
                Query = $@"
                mutation {{
                  users {{
                    delete (id:""{id}"") 
                        }}
                    }}"
            });
            var deleteResponseTwo = await _adminConnection.Value.GraphQlPost(new GraphQLRequest
            {
                Query = $@"
                mutation {{
                  users {{
                    delete (id:""{id}"") 
                        }}
                    }}"
            });
            bool deleteResults = deleteResponse.Data.users.delete;
            bool deleteResults1 = deleteResponseTwo.Data.users.delete;
            // assert

            id.Should().NotBeEmpty();
            updateId.Should().Be(id);
            deleteResults.Should().BeTrue();
            deleteResults1.Should().BeFalse();
        }


        [Test]
        public async Task GraphQl_QueryGuest_ShouldApplyPermissions()
        {
            // arrange
            Setup();
            var userCreate = GetExampleData().First();
            var guest = _guestConnection.Value;

            Action testCall = () =>
            {
                guest.GraphQlPost(new GraphQLRequest
                {
                    Query = $@"
                mutation {{
                  users {{
                    insert (user:{{name:""{userCreate.Name.Substring(0, 10)}"",email:""{userCreate.Email}"",password:""{userCreate.Password}""}}) {{
                                id
                            }}
                        }}
                    }}"
                }).Wait();
            };
            testCall.Should().Throw<Exception>().WithMessage("You are not authorized to run this query.");

            // action
            var graphQlResponse = await guest.GraphQlPost(new GraphQLRequest
            {
                Query = @"
                {
                    users {
                        me {
                            name,activities
                        }
                    }
                }
            "
            });
            object data = graphQlResponse.Data;
            data.Dump("graphQlResponse.Data");

            // assert
            string name = graphQlResponse.Data.users.me.name;
            name.Should().Be("Guest");
        }


        [Test]
        public async Task GraphQl_QueryMeWithLoggedInUser_ShouldReturnCurrentUserName()
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
        public void GraphQl_QueryMeWithNonLoggedInUser_ShouldThrowException()
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
            testCall.Should().Throw<Exception>().WithMessage("Authentication required.")
                .And.ToFirstExceptionOfException().GetType().Name.Should().Be("GraphQlResponseException");
        }


        [Test]
        public async Task GraphQl_RegisterANewUser_ShouldWorkWhenNotLoggedIn()
        {
            // arrange
            Setup();
            var userCreate = GetExampleData().First();
            var newClientNotAuthorized = (CoreDockerClient) _defaultRequestFactory.Value.GetConnection();
            // action
            var insertResponst = await newClientNotAuthorized.GraphQlPost(new GraphQLRequest
            {
                Query = $@"
                mutation {{
                  users {{
                    register (user:{{name:""{userCreate.Name.Mask(10, "...")}"",email:""{userCreate.Email}"",password:""{userCreate.Password}""}}) {{
                                id
                            }}
                        }}
                    }}"
            });
            string id = insertResponst.Data.users.register.id;

            await _adminConnection.Value.GraphQlPost(new GraphQLRequest
            {
                Query = $@"
                mutation {{
                  users {{
                    delete (id:""{id}"") 
                        }}
                    }}"
            });

            // assert

            id.Should().NotBeEmpty();
        }

        [Test]
        public void GraphQl_WhenCalledWithInvalidModel_ShouldThrowExceptionWithSomeDetail()
        {
            // arrange
            Setup();
            var request = new GraphQLRequest
            {
                Query = @"
                mutation {
                  users {
                    insert (user:{name:"""",email:""""}) {
                                id
                            }
                        }
                    }"
            };
            // action
            Action func = () => _adminConnection.Value.GraphQlPost(request).Wait();
            var graphQlResponseException = func.Should().Throw<GraphQlResponseException>().And;
//            graphQlResponseException.GraphQlResponse.Errors.Should().Contain(x=>x.Message == "'Email' is not a valid email address.");
            graphQlResponseException.GraphQlResponse.Errors.Should().Contain(x =>
                x.Message.Contains("'Name' must be between 1 and 150 characters"));

            //graphQlResponseException.GraphQlResponse.Errors.Should().Contain(x=>x.Message == "'Name' must be between 1 and 150 characters. You entered 0 characters.");
        }

        [Test]
        public void Insert_WhenCalledWithInvalidDuplicateEmail_ShouldThrowException()
        {
            // arrange
            Setup();
            var userDetailModel = Builder<UserCreateUpdateModel>.CreateNew().With(x => x.Name = "should fail").Build();
            // action
            Action testCall = () => { _crudController.Insert(userDetailModel).Wait(); };
            // assert
            testCall.Should().Throw<Exception>().WithMessage("'Email' is not a valid email address.");
        }

        [Test]
        public async Task Roles_WhenCalled_ShouldReturnAllRoleInformation()
        {
            // arrange
            Setup();
            // action
            var userModel = await _userApiClient.Roles();
            // assert
            userModel.Count.Should().BeGreaterOrEqualTo(2);
            userModel.Select(x => x.Name).Should().Contain("Admin");
        }

        [Test]
        public async Task WhoAmI_GivenUserData_ShouldReturn()
        {
            // arrange
            Setup();
            // action
            var userModel = await _userApiClient.WhoAmI();
            // assert
            userModel.Should().NotBeNull();
            userModel.Email.Should().StartWith("admin");
        }

        #region Overrides of CrudComponentTestsBase<UserModel,UserCreateUpdateModel,UserReferenceModel>

        protected override EquivalencyAssertionOptions<UserCreateUpdateModel> CompareConfig(
            EquivalencyAssertionOptions<UserCreateUpdateModel> options)
        {
            return base.CompareConfig(options).Excluding(x => x.Password);
        }

        #endregion

        #region Overrides of CrudComponentTestsBase<UserModel,UserCreateUpdateModel>

        protected override IList<UserCreateUpdateModel> GetExampleData()
        {
            var userCreateUpdateModels = Builder<User>.CreateListOfSize(2).WithValidData().Build()
                .DynamicCastTo<List<UserCreateUpdateModel>>();
            userCreateUpdateModels.ForEach(x => x.Password = "passw1");
            return userCreateUpdateModels;
        }

        #endregion
    }
}