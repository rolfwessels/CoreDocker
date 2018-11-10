using System;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using CoreDocker.Utilities.Helpers;

namespace CoreDocker.Core.Tests
{
    [TestFixture]
    public class CodeScannerTests
    {
        private static readonly CodeSanner _codeSanner;


        static CodeScannerTests()
        {
            _codeSanner = new CodeSanner();

        }

        #region Setup/Teardown

        public void Setup()
        {
        }

        #endregion

        [Test]
        public void GetSourcePath_ShouldReturnTheSourceFiles()
        {
            // arrange
            Setup();
            // action
            _codeSanner.GetSourcePath().Should().EndWith("\\src");
            // assert
        }

        [Test]
        [Ignore("not working right")]
        public void FindAllIssues()
        {
            // arrange
            Setup();

            // action
            var fileReports = _codeSanner.ScanNow();
            // assert
            fileReports.SelectMany(x => x.Issues)
                .GroupBy(x => x.Type)
                .ToDictionary(x => x.Key, x => x.Count())
                .Dump("Issue break down");
            foreach (var fileReport in fileReports.OrderBy(x=>x.LinesOfCode))
            {
                Console.Out.WriteLine(fileReport.ToString());
            }
            fileReports.Should().HaveCount(0);

        }

        
    }
}