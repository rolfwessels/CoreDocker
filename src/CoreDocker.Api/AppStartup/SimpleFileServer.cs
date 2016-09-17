using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.AppStartup
{
    public class SimpleFileServer
    {
        private static readonly ILog _log = LogManager.GetLogger<SimpleFileServer>();
//        private static IEnumerable<string> _possibleWebBasePath;


//        public static IEnumerable<string> PossibleWebBasePath
//        {
////            get
////            {
////                if (_possibleWebBasePath != null)
////                    foreach (var path in _possibleWebBasePath)
////                    {
////                        yield return path;
////                    }
////                string combine = Path.Combine(new Uri(Assembly.GetExecutingAssembly().CodeBase).PathAndQuery,
////                    @"..\..\..\");
////                yield return
////                    Path.GetFullPath(Path.Combine(combine, @"CoreDocker.Website2\dist"));
////                yield return
////                    Path.GetFullPath(Path.Combine(combine, @"CoreDocker.Website\build\debug"));
////                yield return
////                    Path.GetFullPath(Path.Combine(combine, @"CoreDocker.Website\dist"));
////                
////            }
////            set { _possibleWebBasePath = value; }
//        }

        public static void Initialize(IServiceCollection services)
        {
//            app.UseStaticFiles();
//            string webBasePath = Settings.Default.WebBasePath;
//            if (!Directory.Exists(webBasePath))
//            {
//                foreach (string path in PossibleWebBasePath)
//                {
//                    if (Directory.Exists(path))
//                    {
//                        _log.Warn("Using alternative path to base path:" + Path.GetFullPath(path));
//                        webBasePath = path;
//                        break;
//                    }
//                    _log.Debug(string.Format("SimpleFileServer:Initialize Tried path {0}", path));
//                }
//            }
//            var options = new FileServerOptions
//            {
//                FileSystem = new PhysicalFileSystem(webBasePath)
//            };
//            appBuilder.UseFileServer(options);
        }

    }
}