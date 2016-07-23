using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CoreDocker.Shared;

namespace CoreDocker.Api.Controllers
{
    [Route(RouteHelper.UserController)]
    public class ConfigController : Controller
    {

        

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet(RouteHelper.WithId)]
        public string Get(int id)
        {
            return "value";
        }
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        [HttpPut(RouteHelper.WithId)]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        [HttpDelete(RouteHelper.WithId)]
        public void Delete(int id)
        {
        }
    }
}