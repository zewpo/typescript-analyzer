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
        public async Task LintProjectTest()
        {
            string mainProjectTsconfig = Path.GetFullPath(@"../../artifacts/tsconfig/multiple/tsconfig.json");
            string[] fileNames = new string[] { mainProjectTsconfig };

            var result = await LinterFactory.Lint(WebLinterVsix.WebLinterPackage.Settings, false, false, fileNames);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(9, result[0].Errors.Count);
        }
    }
}