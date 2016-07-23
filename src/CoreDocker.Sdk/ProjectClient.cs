using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Shared;
using Flurl.Http;
using MainSolutionTemplate.Shared.Interfaces.Shared;
using MainSolutionTemplate.Shared.Models;

namespace CoreDocker.Sdk
{
    public class ProjectClient : ApiClientBase, IProjectControllerActions
    {
        public ProjectClient(CoreDockerClient coreDockerClient)
            : base(coreDockerClient, RouteHelper.Config)
        {
        }
        
        #region Implementation of IConfigClient

        public async Task<List<ProjectModel>> Get()
        {
            return await DefaultUrl()
                .GetAsyncAndLog()
                .ReceiveJson<List<ProjectModel>>();
        }
    
        public Task<dynamic> GetData(string id)
        {
            return DefaultUrl(RouteHelper.WithId.SetParam("id", id))
                .AppendPathSegment("data")
                .GetAsyncAndLog()
                .ReceiveJson<dynamic>();
        }

        public async Task<ProjectModel> Get(string id)
        {
            return await DefaultUrl(RouteHelper.WithId.SetParam("id", id))
             .GetAsyncAndLog()
             .ReceiveJson<ProjectModel>();
        }

        public async Task<ProjectModel> Insert(ProjectCreateUpdateModel model)
        {
            return await DefaultUrl()
               .PutJsonAsyncAndLog(model)
               .ReceiveJson<ProjectModel>();
        }

        public async Task<ProjectModel> Update(string id, ProjectCreateUpdateModel model)
        {
            return await DefaultUrl(RouteHelper.WithId.SetParam("id", id))
               .PostJsonAsyncAndLog(model)
               .ReceiveJson<ProjectModel>();
        }

        public async Task<bool> Delete(string id)
        {
            return await DefaultUrl(RouteHelper.WithId.SetParam("id", id))
           .DeleteAsyncAndLog()
           .ReceiveJson<bool>();
        }

        #endregion
    }

    
}