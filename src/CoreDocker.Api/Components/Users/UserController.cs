using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Api.WebApi.Attributes;
using CoreDocker.Api.WebApi.Controllers;
using CoreDocker.Dal.Models.Enums;
using CoreDocker.Shared;
using CoreDocker.Shared.Interfaces.Base;
using CoreDocker.Shared.Interfaces.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreDocker.Api.Components.Users
{
    /// <summary>
	///     Api controller for managing all the user
	/// </summary>
    [Route(RouteHelper.UserController)]
    public class UserController : Controller, IUserControllerActions, IBaseControllerLookups<UserModel, UserReferenceModel>
    {
	    private readonly UserCommonController _userCommonController;
	    
        public UserController(UserCommonController userCommonController)
        {
            _userCommonController = userCommonController;
        }

        

        /// <summary>
        ///     Returns list of all the users as references
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet,AuthorizeActivity(Activity.ReadUsers)]
        public Task<IEnumerable<UserReferenceModel>> Get()
        {
           
            return _userCommonController.Get(Request.GetQuery());
        }

        /// <summary>
        /// GetCounter all users with their detail.
        /// </summary>
        /// <returns></returns>
        [HttpGet(RouteHelper.WithDetail),AuthorizeActivity(Activity.ReadUsers)]
        public Task<IEnumerable<UserModel>> GetDetail()
		{
            return _userCommonController.GetDetail(Request.GetQuery());
		}


        /// <summary>
		///     Returns a user by his Id.
		/// </summary>
		/// <returns>
		/// </returns>
		[HttpGet(RouteHelper.WithId),AuthorizeActivity(Activity.ReadUsers)]
		public Task<UserModel> GetById(string id)
		{
            return _userCommonController.GetById(id);
		}

	    /// <summary>
	    ///     Updates an instance of the user item.
	    /// </summary>
	    /// <param name="id">The identifier.</param>
	    /// <param name="model">The user.</param>
	    /// <returns>
	    /// </returns>
        [HttpPut(RouteHelper.WithId), AuthorizeActivity(Activity.UpdateUsers)]
		public Task<UserModel> Update(string id, [FromBody] UserCreateUpdateModel model)
		{
            return _userCommonController.Update(id, model);
		}

	    /// <summary>
	    ///     Add a new user
	    /// </summary>
	    /// <param name="model">The user.</param>
	    /// <returns>
	    /// </returns>
        [HttpPost,AuthorizeActivity(Activity.UpdateUsers)]
		public Task<UserModel> Insert([FromBody] UserCreateUpdateModel model)
		{
            return _userCommonController.Insert(model);
		}

	    /// <summary>
	    ///     Deletes the specified user.
	    /// </summary>
	    /// <param name="id">The identifier.</param>
	    /// <returns>
	    /// </returns>
		[HttpDelete(RouteHelper.WithId),AuthorizeActivity(Activity.DeleteUser)]
		public Task<bool> Delete(string id)
		{
            return _userCommonController.Delete(id);
		}

        
        [HttpGet(RouteHelper.UserControllerRoles), AuthorizeActivity(Activity.ReadUsers)]
        public Task<List<RoleModel>> Roles()
        {
            return _userCommonController.Roles();
        }

		#region Other actions

	    /// <summary>
	    /// Registers the specified user.
	    /// </summary>
	    /// <param name="user">The user.</param>
	    /// <returns></returns>
        [HttpPost(RouteHelper.UserControllerRegister), AllowAnonymous]
		public Task<UserModel> Register([FromBody] RegisterModel user)
		{
            return _userCommonController.Register(user);
		}


	    /// <summary>
	    /// Forgot the password sends user an email with his password.
	    /// </summary>
	    /// <param name="email">The email.</param>
	    /// <returns></returns>
        [HttpGet(RouteHelper.UserControllerForgotPassword), AllowAnonymous]
		public Task<bool> ForgotPassword(string email)
		{
            return _userCommonController.ForgotPassword(email);
		}

        /// <summary>
        ///     Return the current user.
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet(RouteHelper.UserControllerWhoAmI), AuthorizeActivity]
        public Task<UserModel> WhoAmI()
        {
            return _userCommonController.WhoAmI();
        }


        #endregion
        
    }
}