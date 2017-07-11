﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebLinter;

namespace WebLinterTest
{
    [TestClass]
    public class TslintWithTsconfigTest
    {
        //private static EnvDTE80.DTE2 dte = null;
        //private static EnvDTE.Solution solution = null;

        //// Clunky, but better than nothing
        //[ClassInitialize]
        //public static void ClassInitialize(TestContext testContext)
        //{
        //    Type type = System.Type.GetTypeFromProgID("VisualStudio.DTE.15.0");
        //    object inst = System.Activator.CreateInstance(type, true);
        //    dte = (EnvDTE80.DTE2)inst;
        //    dte.Solution.Open(Path.GetFullPath(@"../../artifacts/tsconfig/Tsconfig.sln"));
        //    solution = dte.Solution;

        //    Settings settings = new Settings() { TSLintUseTSConfig = true };
        //    WebLinterVsix.WebLinterPackage.Settings = settings;

        //    MessageFilter.Register();
        //    //System.Threading.Thread.Sleep(1000);
        //}

        //[ClassCleanup]
        //public static void ClassCleanup()
        //{
        //    if (solution != null) { solution.Close(); solution = null; }
        //    if (dte != null) dte.Quit();
        //    WebLinterVsix.WebLinterPackage.Settings = null;
        //    MessageFilter.Revoke();
        //}

        [TestMethod, TestCategory("TSLint with tsconfig")]
        public async Task BasicLint()
        {
            // tslint.json is the usual file but has had completed-docs added to allow us to test the type checking

            // Arrange
            string mainProjectTsconfig = Path.GetFullPath(@"../../artifacts/tsconfig/multiple/tsconfig.json");
            string[] fileNames = new string[] { mainProjectTsconfig };

            // Act
            var result = await LinterFactory.Lint(new Settings() { TSLintUseTSConfig = true }, false, false, fileNames);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(15, result[0].Errors.Count);

            // file3.ts is excluded from this tsconfig.json, in spite of being in the VS project.
            // It has errors but they shouldn't appear here
            IList<LintingError> file3Errors = GetErrorsForFile("file3.ts", result[0].Errors);
            Assert.IsTrue(file3Errors.Count == 0);

            // file2.ts is the reverse of the above: it's included in the tsconfig.json file, but is not in the VS project
            // It should have 4 errors, one of them our completed-docs
            IList<LintingError> file2Errors = GetErrorsForFile("file2.ts", result[0].Errors);
            Assert.IsTrue(file2Errors.Count == 4);
            LintingError completedDocsError = file2Errors.First(le => le.ErrorCode == "completed-docs");
            Assert.IsNotNull(completedDocsError);
            Assert.AreEqual(2, completedDocsError.LineNumber);
            Assert.AreEqual(0, completedDocsError.ColumnNumber);
        }

        [TestMethod, TestCategory("TSLint with tsconfig")]
        public async Task LintWithDuplicateErrors()
        {
            // Arrange
            string topTsconfig = Path.GetFullPath(@"../../artifacts/tsconfig/multiple/tsconfig.json");
            string folderbTsconfig = Path.GetFullPath(@"../../artifacts/tsconfig/multiple/b/tsconfig.json");
            string[] fileNames = new string[] { topTsconfig, folderbTsconfig };

            // Act
            var result = await LinterFactory.Lint(new Settings() { TSLintUseTSConfig = true }, false, false, fileNames);

            // Assert
            // file2 is in both tsconfigs.  It has 4 errors.  With the old code we got 8 in the Error List, and here.
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(18, result[0].Errors.Count);

            IList<LintingError> file2Errors = GetErrorsForFile("file2.ts", result[0].Errors);
            Assert.IsTrue(file2Errors.Count == 4);
            LintingError completedDocsError = file2Errors.First(le => le.ErrorCode == "completed-docs");
            Assert.IsNotNull(completedDocsError);
            Assert.AreEqual(2, completedDocsError.LineNumber);
            Assert.AreEqual(0, completedDocsError.ColumnNumber);
        }

        private IList<LintingError> GetErrorsForFile(string fileName, IEnumerable<LintingError> allErrors)
        {
            List<LintingError> result = new List<LintingError>();
            foreach (LintingError lintingError in allErrors)
            {
                if (lintingError.FileName.EndsWith(fileName)) result.Add(lintingError);
            }
            return result;
        }
    }
}
