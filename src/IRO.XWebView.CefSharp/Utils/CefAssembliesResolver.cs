using System;
using System.IO;
using System.Reflection;

namespace IRO.XWebView.CefSharp.Utils
{
    public static class CefAssembliesResolver
    {
        private static string _assembliesDirectory;
        private static bool _defaulAssembliesWasLoaded;

        public static string FindCefAssembliesPath()
        {
            if (_assembliesDirectory == null)
            {
                if (Is64BitProcess() && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "x64\\CefSharp.dll")))
                {
                    _assembliesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "x64");
                }
                else if (!Is64BitProcess() && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "x86\\CefSharp.dll")))
                {
                    _assembliesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "x86");
                }
                else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CefSharp.dll")))
                {
                    _assembliesDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }
                else
                {
                    throw new Exception("Can't find 'CefSharp.dll' assembly in base directory.");
                }
            }
            return _assembliesDirectory;

        }

        //Run this on application startup
        public static void ConfigureCefSharpAssembliesResolve()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;

            FindCefAssembliesPath();
            if (!_defaulAssembliesWasLoaded)
            {

                var defaultAssembliesNames = new[]
                {
                        "CefSharp.Core.dll",
                        "CefSharp.dll",
                        "CefSharp.OffScreen.dll",
                        "CefSharp.Wpf.dll",
                        "CefSharp.WinForms.dll",
                        "CefSharp.BrowserSubprocess.Core.dll"
                };

                foreach (var asmName in defaultAssembliesNames)
                {
                    var filePath = Path.Combine(_assembliesDirectory, asmName);
                    if (File.Exists(filePath))
                    {
                        var loadedAsm = Assembly.LoadFile(filePath);
                    }
                }
                _defaulAssembliesWasLoaded = true;
            }
        }

        public static bool Is64BitProcess()
        {
            return IntPtr.Size == 8;
        }


        // Will attempt to load missing assembly from either x86 or x64 subdir
        // Required by CefSharp to load the unmanaged dependencies when running using AnyCPU
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (!args.Name.StartsWith("CefSharp"))
                return null;

            string requestedAssemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
            Assembly resAssembly = null;
            var filePath = Path.Combine(_assembliesDirectory, requestedAssemblyName);
            if (File.Exists(filePath))
            {
                resAssembly = Assembly.LoadFile(filePath);
            }

            return resAssembly;
        }
    }
}
