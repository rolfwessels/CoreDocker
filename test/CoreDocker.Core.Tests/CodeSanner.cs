﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CoreDocker.Core.Tests
{
    public class CodeSanner
    {
        private Lazy<string[]> _lazy;
        private ICodeSanner[] _runners;

        public CodeSanner()
        {
            _lazy = new Lazy<string[]>(() => Directory.GetFiles(GetSourcePath(), "*.cs",SearchOption.AllDirectories)
                .Where(x=>!x.Contains(@"\obj\")).ToArray());
            _runners = new ICodeSanner[]
            {
                new TestsShouldEndWithFileNameTests(),
                new ClassesWithoutTests(),
            };
        }

        public class ClassesWithoutTests : ICodeSanner
        {
            #region Implementation of ICodeSanner

            public bool ShouldScan(string fileName)
            {
                return !fileName.Contains(".Tests") && !fileName.Contains("AssemblyInfo") && !fileName.Contains("Model") && !fileName.Contains("Constants") && !Path.GetFileName(fileName).StartsWith("I");
            }

            public IEnumerable<Issue> IsFail(string fileName, string[] fileLines, string[] allFiles)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var testclassName = fileNameWithoutExtension + "Tests.cs";
                if (!allFiles.Any(x => x.Contains(testclassName)))
                {
                    yield return
                        new Issue()
                        {
                            Type = GetType().Name,
                            Description = string.Format("Expect the file {0} to have a test class somewhere {1}.", fileNameWithoutExtension, testclassName)
                        };
                }
            }

            #endregion
        }

        public class TestsShouldEndWithFileNameTests : ICodeSanner
        {
            #region Implementation of ICodeSanner

            public bool ShouldScan(string fileName)
            {
                return fileName.Contains(".Tests");
            }

            public IEnumerable<Issue> IsFail(string fileName, string[] fileLines, string[] allFiles)
            {
                var name = Path.GetFileName(fileName)?? "";
                var nameNoExtention = Path.GetFileNameWithoutExtension(fileName)?? "";
                var endsWithTests = name.EndsWith("Test.cs");
                if (endsWithTests)
                {
                    yield return
                        new Issue()
                        {
                            Type = GetType().Name,
                            Description = string.Format("File should be called {0}.", name.Replace("Test.cs", "Tests.cs"))
                        };
                }
                var endsWithTest = name.EndsWith("Tests.cs");
                if (endsWithTest && !fileLines.Any(x => x.Contains(nameNoExtention)))
                {
                    yield return
                        new Issue()
                        {
                            Type = GetType().Name,
                            Description = string.Format("File {0} should contain class {1}.", name, nameNoExtention)
                        };
                }
            }

        

            #endregion
        }

        public string GetSourcePath()
        {
            // todo: Rolf make this dynamic.
            return @"D:\Work\Synced\CoreDocker\src";
        }

        public interface ICodeSanner
        {
            bool ShouldScan(string fileName);
            IEnumerable<Issue> IsFail(string fileName, string[] fileLines, string[] allFiles);
        }

        public List<FileReport> ScanNow()
        {
            var fileReports = new List<FileReport>();
            foreach (var fileName in _lazy.Value)
            {
                if (_runners.Any(x => x.ShouldScan(fileName)))
                {
                    var fileReport = new FileReport() { FileName = fileName, ShortName = fileName.Replace(GetSourcePath(),"") };
                    string[] readAllLines = File.ReadAllLines(fileName);
                    foreach (var runner in _runners.Where(x => x.ShouldScan(fileName)))
                    {
                       
                        var isFail = runner.IsFail(fileName, readAllLines, _lazy.Value);
                        fileReport.Issues.AddRange(isFail);
                    }
                    if (fileReport.Issues.Any())
                    {
                        fileReport.LinesOfCode = readAllLines.Length;
                        fileReports.Add(fileReport);
                    }
                }
            }
            return fileReports;
        }

        public class FileReport
        {
            public FileReport()
            {
                Issues = new List<Issue>();
            }

            public string FileName { get; set; }
            public List<Issue> Issues { get; set; }
            public int LinesOfCode { get; set; }
            public string ShortName { get; set; }

            public override string ToString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(string.Format("FileName: {0} [ lines : {1} , Issues {2}]", ShortName,
                    LinesOfCode, Issues.Count));
                stringBuilder.AppendLine("----------------");
                foreach (var issue in Issues)
                {
                    stringBuilder.AppendLine(issue.ToString());   
                }
                stringBuilder.AppendLine("");
                return stringBuilder.ToString() ;
            }
        }

        public class Issue
        {
            public string Type { get; set; }
            public string Description { get; set; }

            public override string ToString()
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(string.Format("Issue: {0} ", Description));
                stringBuilder.AppendLine("");
                return stringBuilder.ToString();
            }
        }
    }
}