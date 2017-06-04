﻿using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebLinterVsix
{
    internal class LintFilesCommandBase: BuildEventsBase
    {
        internal LintFilesCommandBase(Package package) : base(package) { }

        protected void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            var paths = ProjectHelpers.GetSelectedItemPaths();
            button.Visible = paths.Any(f => string.IsNullOrEmpty(Path.GetExtension(f)) || LinterService.IsFileSupported(f));
        }

        protected async System.Threading.Tasks.Task<bool> LintSelectedFiles(bool fixErrors, bool callSync = false)
        {
            if (!LinterService.IsLinterEnabled)
            {
                WebLinterPackage.Dte.StatusBar.Text = "TSLint is not enabled in Tools/Options";
                return false;
            }
            List<string> files = GetFilesInSelectedItemPaths();
            if (files.Any())
            {
                return await LinterService.LintAsync(showErrorList: true, fixErrors: fixErrors, 
                                                        callSync: callSync, fileNames: files.ToArray());
            }
            else
            {
                WebLinterPackage.Dte.StatusBar.Text = $"No files found to {(fixErrors ? "fix" : "lint")}";
                return false;
            }
        }

        private static List<string> GetFilesInSelectedItemPaths()
        {
            var paths = ProjectHelpers.GetSelectedItemPaths();
            List<string> files = new List<string>();

            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    var children = GetFiles(path, "*.*");
                    files.AddRange(children.Where(c => LinterService.IsFileSupported(c)));
                }
                else if (File.Exists(path) && LinterService.IsFileSupported(path))
                {
                    files.Add(path);
                }
            }

            return files;
        }

        private static List<string> GetFiles(string path, string pattern)
        {
            var files = new List<string>();

            try
            {
                files.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
                foreach (var directory in Directory.GetDirectories(path))
                    files.AddRange(GetFiles(directory, pattern));
            }
            catch (UnauthorizedAccessException) { }

            return files;
        }
    }

}