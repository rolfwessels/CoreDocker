using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Api.WebApi.Attributes;
using CoreDocker.Dal.Models.Enums;
using CoreDocker.Shared;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace CoreDocker.Api.WebApi.Controllers
{

    /// <summary>
	/// Simple ping call. 
	/// </summary>
    [Route(RouteHelper.PingController)]
    public class PingController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger<PingController>();
        /// <summary>
        ///     Returns list of all the projects as references
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet,AuthorizeActivity(Activity.ReadProject)]
        public Task<PingResult> Get()
        {
            _log.Info("Ping - Get");
            return Task.FromResult( new PingResult() {Valid = true});
        }
        
        public class PingResult
        {
            public bool Valid { get; set; }
        }
    }


}
