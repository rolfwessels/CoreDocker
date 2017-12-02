using System;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Dal.Persistance;
using CoreDocker.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CoreDocker.Api.WebApi.Controllers
{

    /// <inheritdoc />
    /// <summary>
    /// Simple ping call. 
    /// </summary>
    [Route(RouteHelper.PingController)]
    public class PingController : Controller
    {
        private readonly IGeneralUnitOfWorkFactory _factory;
        private static readonly string _informationalVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        private readonly string _environmentName;

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
        [HttpGet, AllowAnonymous]
        public Task<PingResult> Get()
        {
            return Task.FromResult( new PingResult() { Version = _informationalVersion , Database = IsDatabaseConnected() , Environment = _environmentName , MachineName = Environment.MachineName });
        }

        /// <summary>
        ///     Returns list of all the projects as references
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet (RouteHelper.PingControllerHealthCheck), AllowAnonymous]
        public Task<PingResult> GetHealthCheck()
        {
            return Task.FromResult(new PingResult() { Version = _informationalVersion, Database = "Unknown.", Environment = _environmentName, MachineName = Environment.MachineName });
        }

        private string IsDatabaseConnected()
        {
            try
            {
                _factory.GetConnection();
                return "Connected";
            }
            catch (Exception e)
            {
                return "Error:"+e.Message;
            }
        }

        public class PingResult
        {
            public string Environment { get; set; }
            public string Version { get; set; }
            public string Database { get; set; }
            public string MachineName { get; set; }
        }
    }


}
