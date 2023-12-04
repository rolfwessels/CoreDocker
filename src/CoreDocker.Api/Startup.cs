using System.Diagnostics;
using System.Reflection;
using CoreDocker.Core.Framework.Settings;

namespace CoreDocker.Api
{
    public class EnvHelper
    {
        public static string InformationalVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var productVersion = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
            return $"{productVersion}";
        }

        
    }
}