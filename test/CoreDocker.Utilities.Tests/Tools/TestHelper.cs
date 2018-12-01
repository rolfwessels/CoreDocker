using System;
using System.Threading.Tasks;
using Moq.Language.Flow;

namespace CoreDocker.Utilities.Tests.Tools
{
    public static class TestHelper
    {
        private static readonly Lazy<string> LazySourcePath = new Lazy<string>(SourceBasePath);

        public static void Returns<T1, T2>(this ISetup<T1, Task<T2>> setup, T2 dal) where T1 : class
        {
            setup.Returns(Task.FromResult(dal));
        }

        public static string GetSourceBasePath()
        {
            return LazySourcePath.Value;
        }

        #region Private Methods

        private static string SourceBasePath()
        {
            return @"D:\Work\home\pception\src\Pception.Website\dist";
//            var path = Path.GetFullPath(NCrunchEnvironment.GetOriginalSolutionPath() ??
//                                        new Uri(Assembly.GetAssembly(typeof(TestHelper)).CodeBase, UriKind.Absolute).LocalPath);
//            while (Path.GetFileName(path) != "src")
//            {
//                var directoryName = Path.GetDirectoryName(path);
//                if (directoryName == null) break;
//                path = directoryName;
//
//            }
//            return path;
        }

        #endregion
    }
}