using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared;
using CoreDocker.Shared.Models.Projects;
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
                        all {
                            ...projectData
                        }
                    }
                }"
            };
            var response = await CoreDockerClient.GraphQlPost(request);
            return CastHelper.DynamicCastTo<List<ProjectModel>>(response.Data.projects.all);
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

        public async Task<ProjectModel> Create(ProjectCreateUpdateModel project)
        {
            var response = await CoreDockerClient.GraphQlPost(new GraphQLRequest
            {
                Query = GraphQlFragments.Project + @"
                mutation ($name: String!) {
                  projects {
                    create(project: {name: $name}) {
                      ...projectData
                    }
                  }
                }",
                Variables = new { project.Name }
            });
            return CastHelper.DynamicCastTo<ProjectModel>(response.Data.projects.create);
        }
        
        public async Task<ProjectModel> Update(string id,ProjectCreateUpdateModel project)
        {
            var response = await CoreDockerClient.GraphQlPost(new GraphQLRequest
            {
                Query = GraphQlFragments.Project + @"
                mutation ($id: String!, $name: String!) {
                  projects {
                    update(id: $id, project: {name: $name}) {
                      ...projectData
                    }
                  }
                }",
                Variables = new { id, project.Name }
            });
            
            return CastHelper.DynamicCastTo<ProjectModel>(response.Data.projects.update);
        }

        public async Task<bool> Remove(string id)
        {
            var response = await CoreDockerClient.GraphQlPost(new GraphQLRequest
            {
                Query = @"
                mutation ($id: String!) {
                  projects {
                    remove(id: $id)
                  }
                }",
                Variables = new { id }
            });

            return CastHelper.DynamicCastTo<bool>(response.Data.projects.remove);
        }


    }
}