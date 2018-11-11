using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared;
using CoreDocker.Shared.Models.Users;
using CoreDocker.Utilities.Helpers;
using GraphQL.Common.Request;

namespace CoreDocker.Sdk.RestApi.Clients
{
    public class UserApiClient : BaseApiClient
    {
        public UserApiClient(CoreDockerClient dockerClient)
            : base(dockerClient, RouteHelper.UserController)
        {
        }

        #region Implementation of IUserControllerActions

        public async Task<List<UserModel>> All()
        {
            var request = new GraphQLRequest
            {
                Query = Fragments.User + @"{
                    users {
                        all {
                            ...userData
                        }
                    }
                }"
            };
            var response = await CoreDockerClient.GraphQlPost(request);
            return CastHelper.DynamicCastTo<List<UserModel>>(response.Data.users.all);
        }

        public async Task<UserModel> ById(string id)
        {
            var request = new GraphQLRequest
            {
                 
                Query = Fragments.User + @"query ($id: String!) {
                  users {
                    byId(id: $id) {
                      ...userData
                    }
                  }
                }",
                Variables = new {id}
            };
            var response = await CoreDockerClient.GraphQlPost(request);
            return CastHelper.DynamicCastTo<UserModel>(response.Data.users.byId);
        }

        public async Task<UserModel> Me()
        {
            var request = new GraphQLRequest
            {
                Query = Fragments.User+ @"{
                    users {
                        me {
                            ...userData
                        }
                    }
                }"};
            var response = await CoreDockerClient.GraphQlPost(request);
            return CastHelper.DynamicCastTo<UserModel>(response.Data.users.me);
        }

        public async Task<UserModel> Create(UserCreateUpdateModel user)
        {
            var response = await CoreDockerClient.GraphQlPost(new GraphQLRequest
            {
                Query = Fragments.User + @"
                mutation ($name: String!, $email: String!, $roles: [String], $password: String) {
                  users {
                    insert(user: {name: $name, email: $email, roles: $roles, password: $password}) {
                      ...userData
                    }
                  }
                }",
                Variables = new { user.Name, user.Email, user.Roles, user.Password }
            });
            return CastHelper.DynamicCastTo<UserModel>(response.Data.users.insert);
        }

        public async Task<UserModel> Register(RegisterModel user)
        {
            var response = await CoreDockerClient.GraphQlPost(new GraphQLRequest
            {
                Query = Fragments.User + @"
                mutation ($name: String!, $email: String!, $password: String!) {
                  users {
                    register(user: {name: $name, email: $email, password: $password}) {
                      ...userData
                    }
                  }
                }",
                Variables = new { user.Name, user.Email,  user.Password }
            });
            return CastHelper.DynamicCastTo<UserModel>(response.Data.users.register);
        }

        public async Task<UserModel> UpdateIt(string id,UserCreateUpdateModel user)
        {
            var response = await CoreDockerClient.GraphQlPost(new GraphQLRequest
            {
                Query = Fragments.User + @"
                mutation ($id: String!, $name: String!, $email: String!, $roles: [String], $password: String) {
                  users {
                    update(id: $id, user: {name: $name, email: $email, roles: $roles, password: $password}) {
                      ...userData
                    }
                  }
                }",
                Variables = new { id, user.Name, user.Email, user.Roles, user.Password}
            });
            
            return CastHelper.DynamicCastTo<UserModel>(response.Data.users.update);
        }

        public async Task<bool> Remove(string id)
        {
            var response = await CoreDockerClient.GraphQlPost(new GraphQLRequest
            {
                Query = @"
                mutation ($id: String!) {
                  users {
                    delete(id: $id)
                  }
                }",
                Variables = new { id }
            });

            return CastHelper.DynamicCastTo<bool>(response.Data.users.delete);
        }


        public async Task<List<RoleModel>> Roles()
        {
            var response = await CoreDockerClient.GraphQlPost(new GraphQLRequest
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
            return CastHelper.DynamicCastTo<List<RoleModel>>(response.Data.users.roles);
        }


        #endregion
    }
}