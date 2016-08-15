using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace CoreDocker.Sdk
{
    public static class FlurlHelper
    {

        public static Url SetRoute(this Url appendPathSegment, string route)
        {
            return appendPathSegment.AppendPathSegment(route);
        }
        public static Url AppendPathSegment(this Url appendPathSegment, string value,string key)
        {
            return appendPathSegment.ToString().Replace("%7B" + value + "%7D", key);
        }

        public static async Task<HttpResponseMessage> GetAsyncAndLog(this Url appendPathSegment)
        {
            return await AsyncAndLog(appendPathSegment, () => appendPathSegment.GetAsync(), "GET");
        }

        public static async Task<HttpResponseMessage> PostJsonAsyncAndLog(this Url appendPathSegment, object post)
        {
            return await AsyncAndLog(appendPathSegment, () => appendPathSegment.PostJsonAsync(post), "POST");
        }
        
        public static async Task<HttpResponseMessage> PutJsonAsyncAndLog(this Url appendPathSegment, object post)
        {
            return await AsyncAndLog(appendPathSegment, () => appendPathSegment.PutJsonAsync(post), "PUT");
        }

        public static async Task<HttpResponseMessage> DeleteAsyncAndLog(this Url appendPathSegment)
        {
            return await AsyncAndLog(appendPathSegment, () => appendPathSegment.DeleteAsync(), "DELETE");
        }

        private static async Task<HttpResponseMessage> AsyncAndLog(Url appendPathSegment, Func<Task<HttpResponseMessage>> call, string args)
        {
            if (Log != null) Log(string.Format("Call {1} {0}", appendPathSegment,args));
            try
            {
                var asyncAndLog = await call();
                if (Log != null)
                {
                    var readAsStringAsync = await asyncAndLog.Content.ReadAsStringAsync();
                    Log(string.Format("Resonse {0} {1}", appendPathSegment, readAsStringAsync));
                }
                return asyncAndLog;
            }
            catch (FlurlHttpException e)
            {
                if (Log != null)
                {
                    Log(string.Format("Resonse {0} {1}", appendPathSegment, e.GetResponseString()));
                }
                throw;
            }
        }

        

        
        public static Action<string> Log { get; set; }
    }
}