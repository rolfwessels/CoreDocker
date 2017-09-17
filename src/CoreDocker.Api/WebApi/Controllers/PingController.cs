using System;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Api.WebApi.Attributes;
using CoreDocker.Dal.Models.Enums;
using CoreDocker.Dal.Persistance;
using CoreDocker.Shared;
using log4net;
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
        private static readonly ILog _log = LogManager.GetLogger<PingController>();

        public PingController(IGeneralUnitOfWorkFactory factory)
        {
            _factory = factory;
        }

        public static string Env { get; set; } = "Develop";

        /// <summary>
        ///     Returns list of all the projects as references
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet,AuthorizeActivity(Activity.ReadProject)]
        public Task<PingResult> Get()
        {
            var informationalVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            return Task.FromResult( new PingResult() { Version = informationalVersion , Database = IsDatabaseConnected() , Environment = Env });
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
        }
    }


}
