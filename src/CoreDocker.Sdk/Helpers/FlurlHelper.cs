using System;
using System.Net.Http;
using System.Threading.Tasks;
using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared.Models;
using Flurl;
using Flurl.Http;

namespace CoreDocker.Sdk.Helpers
{
    public static class FlurlHelper
    {

//        public static Url SetRoute(this Url appendPathSegment, string route)
//        {
//            return appendPathSegment.AppendPathSegment(route);
//        }
//        public static Url AppendPathSegment(this Url appendPathSegment, string value,string key)
//        {
//            return appendPathSegment.ToString().Replace("%7B" + value + "%7D", key);
//        }
//
//        public static async Task<HttpResponseMessage> GetAsyncAndLog(this Url appendPathSegment)
//        {
//            return await AsyncAndLog(appendPathSegment, () => appendPathSegment.WithHeader("Accept", "application/json").GetAsync(), "GET");
//        }
//
//        public static async Task<HttpResponseMessage> PostJsonAsyncAndLog(this Url appendPathSegment, object post)
//        {
//            return await AsyncAndLog(appendPathSegment, () => appendPathSegment.PostJsonAsync(post), "POST", post);
//        }
//        
//        public static async Task<HttpResponseMessage> PutJsonAsyncAndLog(this Url appendPathSegment, object post)
//        {
//            return await AsyncAndLog(appendPathSegment, () => appendPathSegment.PutJsonAsync(post), "PUT", post);
//        }
//
//        public static async Task<HttpResponseMessage> DeleteAsyncAndLog(this Url appendPathSegment)
//        {
//            return await AsyncAndLog(appendPathSegment, () => appendPathSegment.WithHeader("Accept", "application/json").DeleteAsync(), "DELETE" );
//        }
//
//        private static async Task<HttpResponseMessage> AsyncAndLog(Url appendPathSegment, Func<Task<HttpResponseMessage>> call, string args, object data = null)
//        {
//
//            var serializedData = "";
//            if (data != null) FlurlHttp.Configure(x => serializedData = x.JsonSerializer.Serialize(data));
//
//            if (Log != null) Log(string.Format("Call {1} {0} [{2}]", appendPathSegment,args , serializedData));
//            try
//            {
//                var asyncAndLog = await call();
//                if (Log != null)
//                {
//                    var readAsStringAsync = await asyncAndLog.Content.ReadAsStringAsync();
//                    Log(string.Format("Response {0} {1}", appendPathSegment, readAsStringAsync));
//                }
//                return asyncAndLog;
//            }
//            catch (FlurlHttpException e)
//            {
//                var responseString = e.GetResponseString();
//                if (Log != null)
//                {
//                    
//                    LogError(string.Format("Response {0} {1}", appendPathSegment, responseString));
//                }
//                ErrorMessage message = null;
//                if (responseString != null) FlurlHttp.Configure(x => message = x.JsonSerializer.Deserialize<ErrorMessage>(responseString));
//                throw new RestClientException(message, e);
//            }
//        }
//
//        
//
//        
//        public static Action<string> Log { get; set; }
//        public static Action<string> LogError { get; set; }
    }
}