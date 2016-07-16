using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CoreDocker.Utilities.Helpers
{
    public static class EmbededResourceHelper
    {
        public static string ReadResource(string resourceName, Assembly getExecutingAssembly)
        {
            var assembly = getExecutingAssembly;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new ArgumentException(string.Format("{0} resource does not exist in {1} assembly.",
                        resourceName, getExecutingAssembly.FullName.Split(',').First()));
                return stream.ReadToString();
            }
        }

        public static string ReadResource(string resourceName, Type type)
        {
            return ReadResource(resourceName, type.GetTypeInfo().Assembly);
        }
    }
}