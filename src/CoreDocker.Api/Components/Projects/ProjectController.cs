using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Api.WebApi.Attributes;
using CoreDocker.Api.WebApi.Controllers;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Shared;
using CoreDocker.Shared.Interfaces.Base;
using CoreDocker.Shared.Interfaces.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Projects;
using Microsoft.AspNetCore.Mvc;

namespace CoreDocker.Api.Components.Projects
{

    /// <summary>
	///     Api controller for managing all the project
	/// </summary>
    [Route(RouteHelper.ProjectController)]
    public class ProjectController : Controller, IProjectControllerActions, IBaseControllerLookups<ProjectModel, ProjectReferenceModel>
    {
	    private readonly ProjectCommonController _projectCommonController;
	    
        public ProjectController(ProjectCommonController projectCommonController)
        {
            _projectCommonController = projectCommonController;
        }

        /// <summary>
        ///     Returns list of all the projects as references
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet,AuthorizeActivity(Activity.ReadProject)]
        public Task<IEnumerable<ProjectReferenceModel>> Get()
        {   
            return _projectCommonController.Get(Request.GetQuery());
        }

        /// <summary>
        /// GetCounter all projects with their detail.
        /// </summary>
        /// <returns></returns>
        [HttpGet(RouteHelper.WithDetail),AuthorizeActivity(Activity.ReadProject)]
        public Task<IEnumerable<ProjectModel>> GetDetail()
		{
		    return _projectCommonController.GetDetail(Request.GetQuery());
		}


        /// <summary>
		///     Returns a project by his Id.
		/// </summary>
		/// <returns>
		/// </returns>
		[HttpGet(RouteHelper.WithId),AuthorizeActivity(Activity.ReadProject)]
		public Task<ProjectModel> GetById(string id)
		{
            return _projectCommonController.GetById(id);
		}

	    /// <summary>
	    ///     Updates an instance of the project item.
	    /// </summary>
	    /// <param name="id">The identifier.</param>
	    /// <param name="model">The project.</param>
	    /// <returns>
	    /// </returns>
		[HttpPut(RouteHelper.WithId),AuthorizeActivity(Activity.UpdateProject) ]
		public Task<ProjectModel> Update(string id, [FromBody] ProjectCreateUpdateModel model)
		{
            return _projectCommonController.Update(id, model);
		}

	    /// <summary>
	    ///     Add a new project
	    /// </summary>
	    /// <param name="model">The project.</param>
	    /// <returns>
	    /// </returns>
        [HttpPost,AuthorizeActivity(Activity.UpdateProject)]
		public Task<ProjectModel> Insert([FromBody] ProjectCreateUpdateModel model)
		{
            return _projectCommonController.Insert(model);
		}

	    /// <summary>
	    ///     Deletes the specified project.
	    /// </summary>
	    /// <param name="id">The identifier.</param>
	    /// <returns>
	    /// </returns>
		[HttpDelete(RouteHelper.WithId),AuthorizeActivity(Activity.DeleteProject)]
		public Task<bool> Delete(string id)
		{
            return _projectCommonController.Delete(id);
		}



		
	}
}

/* scaffolding
[{
      "FileName": "RouteHelper.cs",
      "Indexline": "UserControllerForgotPassword",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        null,
        "public const string ProjectController = ApiPrefix + \"project\";"
      ]
    },
    {
      "FileName": "Activity.cs",
      "Indexline": "DeleteProject",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        null,
        "ReadProject = x00,",
        "UpdateProject = x01,",
        "InsertProject = x02,",
        "DeleteProject = x03,",
        "SubscribeProject = x04,"
      ]
    }]
scaffolding */