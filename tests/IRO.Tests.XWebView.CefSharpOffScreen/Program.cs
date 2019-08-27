using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using IRO.Tests.XWebView.Core;
using IRO.Tests.XWebView.Core.Tests;
using IRO.XWebView.CefSharp.OffScreen.Utils;
using IRO.XWebView.CefSharp.OffScreen.Providers;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.Core.Utils;

namespace IRO.Tests.XWebView.CefSharpOffScreen
{
    class Program
    {
        static void Main(string[] args)
        {
            //!Please use x86|x64 configuratian.
            //This project used only to test CefSharpThreadSyncInvoker,
            //other tests you can find in IRO.Tests.XWebView.CefSharpWpf.
            Task.Run(async () =>
            {
                var env = new ConsoleTestingEnvironment();
                try
                {
                    AppDomain.CurrentDomain.AssemblyResolve += Resolver;
                    InitializeCefSharp();
                    ThreadSync.Init(new CefSharpThreadSyncInvoker());
                    var provider = new OffScreenCefSharpXWebViewProvider();
                    var test = new TestScreenshotViaJs();
                    var appConfig = new TestAppSetupConfigs()
                    {
                        ContentPath = Environment.CurrentDirectory
                    };
                    await test.RunTest(provider, env, appConfig);
                }
                catch(Exception ex)
                {
                    env.Error(ex.ToString());
                }
            });
            while (true)
            {
                Console.ReadLine();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void InitializeCefSharp()
        {
            var settings = new CefSettings();
            settings.BrowserSubprocessPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                Environment.Is64BitProcess ? "x64" : "x86",
                "CefSharp.BrowserSubprocess.exe"
            );
            CefHelpers.AddDefaultSettings(settings);
            settings.RemoteDebuggingPort = 9222;
            Cef.Initialize(settings, false, browserProcessHandler: null);
        }

        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }
    }


}
