using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Projects;
using CoreDocker.Shared.Models.Users;
using CoreDocker.Utilities.Helpers;
using GraphQL.Common.Request;

namespace CoreDocker.Sdk.RestApi.Clients
{
    public class ProjectApiClient : BaseApiClient
    {
        public ProjectApiClient(CoreDockerClient dockerClient)
            : base(dockerClient, RouteHelper.ProjectController)
        {
        }

        public async Task<List<ProjectModel>> All()
        {
            var request = new GraphQLRequest
            {
                Query = GraphQlFragments.Project + @"{
                    projects {
                        list {
                            ...projectData
                        }
                    }
                }"
            };
            var response = await CoreDockerClient.GraphQlPost(request);
            return CastHelper.DynamicCastTo<List<ProjectModel>>(response.Data.projects.list);
        }

        public async Task<PagedListModel<ProjectModel>> Paged(int? first = null)
        {
            var request = new GraphQLRequest
            {
                Query = GraphQlFragments.Project + @"query ($first: Int){
                    projects {
                        paged(first:$first) {
                            count,
                            items {...projectData}
                        }
                    }
                }",
                Variables = new {first}
            };
            var response = await CoreDockerClient.GraphQlPost(request);
            return CastHelper.DynamicCastTo<PagedListModel<ProjectModel>>(response.Data.projects.paged);
        }

        public async Task<ProjectModel> ById(string id)
        {
            var request = new GraphQLRequest
            {
                Query = GraphQlFragments.Project + @"query ($id: String!) {
                  projects {
                    byId(id: $id) {
                      ...projectData
                    }
                  }
                }",
                Variables = new {id}
            };
            var response = await CoreDockerClient.GraphQlPost(request);
            return CastHelper.DynamicCastTo<ProjectModel>(response.Data.projects.byId);
        }

        public async Task<CommandResultModel> Create(ProjectCreateUpdateModel project)
        {
            var response = await CoreDockerClient.GraphQlPost(new GraphQLRequest
            {
                Query = GraphQlFragments.CommandResult + @"
                mutation ($name: String!) {
                  projects {
                    create(project: {name: $name}) {
                      ...commandResultData
                    }
                  }
                }",
                Variables = new {project.Name}
            });
            return CastHelper.DynamicCastTo<CommandResultModel>(response.Data.projects.create);
        }

        public async Task<CommandResultModel> Update(string id, ProjectCreateUpdateModel project)
        {
            var response = await CoreDockerClient.GraphQlPost(new GraphQLRequest
            {
                Query = GraphQlFragments.CommandResult + @"
                mutation ($id: String!, $name: String!) {
                  projects {
                    update(id: $id, project: {name: $name}) {
                      ...commandResultData
                    }
                  }
                }",
                Variables = new {id, project.Name}
            });

            return CastHelper.DynamicCastTo<CommandResultModel>(response.Data.projects.update);
        }

        public async Task<CommandResultModel> Remove(string id)
        {
            var response = await CoreDockerClient.GraphQlPost(new GraphQLRequest
            {
                Query = GraphQlFragments.CommandResult + @"
                mutation ($id: String!) {
                  projects {
                    remove(id: $id) {
                        ...commandResultData
                    }
                  }
                }",
                Variables = new {id}
            });

            return CastHelper.DynamicCastTo<CommandResultModel>(response.Data.projects.remove);
        }
    }
}