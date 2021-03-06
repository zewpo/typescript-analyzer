﻿// Modifications Copyright Rich Newman 2017
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics.CodeAnalysis;

namespace WebLinterVsix
{
    public static class Logger
    {
        private static IVsOutputWindowPane pane;
        private static object _syncRoot = new object();
        private static IServiceProvider _provider;
        private static string _name;

        public static void Initialize(IServiceProvider provider, string name)
        {
            _provider = provider;
            _name = name;
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.Shell.Interop.IVsOutputWindowPane.OutputString(System.String)")]
        public static void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            try
            {
                if (EnsurePane())
                {
                    pane.OutputString(DateTime.Now.ToString() + ": " + message + Environment.NewLine);
                }
            }
            catch
            {
                // Do nothing
            }
        }

        public static void Log(Exception ex)
        {
            try
            {
                if (ex != null)
                {
                    Log(ex.ToString());
                }
            }
            catch (Exception) { }
        }

        public static void LogAndWarn(Exception ex)
        {
            try
            {
                if (ex != null)
                {
                    Log(ex.ToString());
                    string warning = "A TypeScript Analyzer error occurred. See Output window for more details.";
                    WebLinterPackage.Dte.StatusBar.Text = warning;
                }
            }
            catch (Exception) { }
        }

        private static bool EnsurePane()
        {
            if (pane == null)
            {
                Guid guid = Guid.NewGuid();
                IVsOutputWindow output = (IVsOutputWindow)_provider.GetService(typeof(SVsOutputWindow));
                output.CreatePane(ref guid, _name, 1, 1);
                output.GetPane(ref guid, out pane);
            }

            return pane != null;
        }
    }
}
