using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Projects;
using CoreDocker.Shared.Models.Users;
using CoreDocker.Utilities.Helpers;
using GraphQL;

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
                        paged {
                            items {
                              ...projectData
                            }   
                        }
                    }
                }"
            };
            var response = await CoreDockerClient.Post<Response>(request);

            return response.Data.Projects.Paged.Items;
        }

        public async Task<PagedListModel<ProjectModel>> Paged(int? first = null)
        {
            var request = new GraphQLRequest
            {
                Query = GraphQlFragments.Project + @"query ($first: Int){
                    projects {
                        paged(first:$first, includeCount: true) {
                            count,
                            items {...projectData}
                        }
                    }
                }",
                Variables = new {first}
            };
            var response = await CoreDockerClient.Post<Response>(request);
            return response.Data.Projects.Paged;
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
            var response = await CoreDockerClient.Post<Response>(request);
            return response.Data.Projects.ById;
        }

        public async Task<CommandResultModel> Create(ProjectCreateUpdateModel project)
        {
            var response = await CoreDockerClient.Post<Response>(new GraphQLRequest
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
            return response.Data.Projects.Create;
        }

        public async Task<CommandResultModel> Update(string id, ProjectCreateUpdateModel project)
        {
            var response = await CoreDockerClient.Post<Response>(new GraphQLRequest
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

            return response.Data.Projects.Update;
        }

        public async Task<CommandResultModel> Remove(string id)
        {
            var response = await CoreDockerClient.Post<Response>(new GraphQLRequest
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

            return response.Data.Projects.Remove;
        }


        class Response
        {
            public ResponseData Projects { get; set; }

            public class ResponseData
            {
                public PagedListModel<ProjectModel> Paged { get; set; }
                public ProjectModel ById { get; set; }
                public CommandResultModel Create { get; set; }
                public CommandResultModel Update { get; set; }
                public CommandResultModel Remove { get; set; }

            }
        }
    }
}
