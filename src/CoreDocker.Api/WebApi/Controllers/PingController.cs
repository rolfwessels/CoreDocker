using System;
using System.Threading.Tasks;
using CoreDocker.Dal.Persistence;
using CoreDocker.Shared;
using CoreDocker.Shared.Models.Ping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CoreDocker.Api.WebApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    ///     Simple ping call.
    /// </summary>
    [Route(RouteHelper.PingController)]
    public class PingController : Controller
    {
        private static readonly string _informationalVersion = Startup.InformationalVersion();
        private readonly string _environmentName;
        private readonly IGeneralUnitOfWorkFactory _factory;

        public PingController(IGeneralUnitOfWorkFactory factory, IHostingEnvironment environment)
        {
            _factory = factory;
            _environmentName = environment.EnvironmentName;
        }

        /// <summary>
        ///     Returns list of all the projects as references
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        public Task<PingModel> Get()
        {
            return Task.FromResult(new PingModel
            {
                Version = _informationalVersion,
                Database = IsDatabaseConnected(),
                Environment = _environmentName,
                MachineName = Environment.MachineName
            });
        }

        /// <summary>
        ///     Returns list of all the projects as references
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet(RouteHelper.PingControllerHealthCheck)]
        [AllowAnonymous]
        public Task<PingModel> GetHealthCheck()
        {
            return Task.FromResult(new PingModel
            {
                Version = _informationalVersion,
                Database = "Unknown.",
                Environment = _environmentName,
                MachineName = Environment.MachineName
            });
        }

        #region Private Methods

        private string IsDatabaseConnected()
        {
            try
            {
                _factory.GetConnection();
                return "Connected";
            }
            catch (Exception e)
            {
                return "Error:" + e.Message;
            }
        }

        #endregion
    }
}