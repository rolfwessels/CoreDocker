using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Users;
using Bumbershoot.Utilities.Helpers;
using GraphQL;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace CoreDocker.Sdk.RestApi.Clients
{
    public class UserApiClient : BaseApiClient
    {
        public UserApiClient(CoreDockerClient dockerClient)
            : base(dockerClient, RouteHelper.UserController)
        {
        }

        public async Task<List<UserModel>> List()
        {
            var request = new GraphQLRequest
            {
                Query = GraphQlFragments.User + @"{
                    users {
                        paged() {
                            items {...userData}
                        }
                    }
                }"
            };
            var response = await CoreDockerClient.Post<Response>(request);
            return response.Data.Users.Paged.Items;
        }

        public async Task<UserModel> ById(string id)
        {
            var request = new GraphQLRequest
            {
                Query = GraphQlFragments.User + @"query ($id: String!) {
                  users {
                    byId(id: $id) {
                      ...userData
                    }
                  }
                }",
                Variables = new {id}
            };
            var response = await CoreDockerClient.Post<Response>(request);
            return response.Data.Users.ById;
        }

        public async Task<UserModel> Me()
        {
            var request = new GraphQLRequest
            {
                Query = GraphQlFragments.User + @"{
                    users {
                        me {
                            ...userData
                        }
                    }
                }"
            };
            var response = await CoreDockerClient.Post<Response>(request);
            return response.Data.Users.Me;
        }

        public async Task<CommandResultModel> Create(UserCreateUpdateModel user)
        {
            var response = await CoreDockerClient.Post<Response>(new GraphQLRequest
            {
                Query = GraphQlFragments.CommandResult + @"
                mutation ($name: String!, $email: String!, $roles: [String!]!, $password: String) {
                  users {
                    create(user: {name: $name, email: $email, roles: $roles, password: $password}) {
                      ...commandResultData
                    }
                  }
                }",
                Variables = new {user.Name, user.Email, user.Roles, user.Password}
            });
            return response.Data.Users.Create;
        }

        public async Task<CommandResultModel> Register(RegisterModel user)
        {
            var response = await CoreDockerClient.Post<Response>(new GraphQLRequest
            {
                Query = GraphQlFragments.CommandResult + @"
                mutation ($name: String!, $email: String!, $password: String!) {
                  users {
                    register(user: {name: $name, email: $email, password: $password}) {
                      ...commandResultData
                    }
                  }
                }",
                Variables = new {user.Name, user.Email, user.Password}
            });
            return response.Data.Users.Register;
        }

        public async Task<CommandResultModel> Update(string id, UserCreateUpdateModel user)
        {
            var response = await CoreDockerClient.Post<Response>(new GraphQLRequest
            {
                Query = GraphQlFragments.CommandResult + @"
                mutation ($id: String!, $name: String!, $email: String!, $roles: [String!]!, $password: String) {
                  users {
                    update(id: $id, user: {name: $name, email: $email, roles: $roles, password: $password}) {
                      ...commandResultData
                    }
                  }
                }",
                Variables = new {id, user.Name, user.Email, user.Roles, user.Password}
            });

            return response.Data.Users.Update;
        }

        public async Task<CommandResultModel> Remove(string id)
        {
            var response = await CoreDockerClient.Post<Response>(new GraphQLRequest
            {
                Query = GraphQlFragments.CommandResult + @"
                mutation ($id: String!) {
                  users {
                    remove(id: $id) {
                        ...commandResultData
                    }
                  }
                }",
                Variables = new {id}
            });

            return response.Data.Users.Remove;
        }


        public async Task<List<RoleModel>> Roles()
        {
            var response = await CoreDockerClient.Post<Response>(new GraphQLRequest
            {
                Query = @"{
                  users {
                    roles {
                      name
                      activities
                    }
                  }
                }"
            });
            return response.Data.Users.Roles;
        }


        public async Task<PagedListModel<UserModel>> Paged(int? first = null)
        {
            var request = new GraphQLRequest
            {
                Query = GraphQlFragments.User + @"query ($first: Int){
                    users {
                        paged(first:$first, includeCount: true) {
                            count,
                            items {...userData}
                        }
                    }
                }",
                Variables = new {first}
            };
            var response = await CoreDockerClient.Post<Response>(request);
            return response.Data.Users.Paged;
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class Response
        {
            public ResponseData Users { get; set; } = null!;

            public class ResponseData
            {
                public CommandResultModel Register { get; set; } = null!;
                public UserModel Me { get; set; } = null!;
                public List<RoleModel> Roles { get; set; } = null!;
                public PagedListModel<UserModel> Paged { get; set; } = null!;
                public UserModel ById { get; set; } = null!;
                public CommandResultModel Create { get; set; } = null!;
                public CommandResultModel Update { get; set; } = null!;
                public CommandResultModel Remove { get; set; } = null!;
            }
        }
    }
}