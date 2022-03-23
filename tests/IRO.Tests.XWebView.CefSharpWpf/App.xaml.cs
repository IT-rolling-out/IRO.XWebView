using CefSharp;
using CefSharp.Wpf;
using IRO.Tests.XWebView.CefSharpWpf.JsInterfaces;
using IRO.Tests.XWebView.Core;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.CefSharp.Wpf.Utils;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IRO.Tests.XWebView.CefSharpWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            CefAssembliesResolver.ConfigureCefSharpAssembliesResolve();
            InitializeCefSharp();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            var settings = new CefSettings
            {
                BrowserSubprocessPath = Path.Combine(
                    CefAssembliesResolver.FindCefAssembliesPath(),
                    "CefSharp.BrowserSubprocess.exe"
                    )
            };
            CefHelpers.AddDefaultSettings(settings);
            settings.RemoteDebuggingPort = 9222;
            Cef.Initialize(settings, false, browserProcessHandler: null);
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                Cef.Shutdown();
            };
        }

        [STAThread]
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Task.Run(async () =>
            {
                XWebViewThreadSync.Init(new WpfThreadSyncInvoker());

                var testEnv = new WpfTestEnvironment();
                try
                {
                    var provider = new TestXWebViewProvider();
                    var mainXWV = await provider.Resolve(XWebViewVisibility.Visible);
                    mainXWV.LoadFinished += async (s, args) =>
                    {
                        try
                        {
                            ((IXWebView)s).SetPageZoomLevel(0.8);
                        }
                        catch { }
                    };

                    mainXWV.BindToJs(new WpfNativeJsInterface(), "WpfNative");

                    var appConfigs = new TestAppSetupConfigs
                    {
                        MainXWebView = mainXWV,
                        Provider = provider,
                        TestingEnvironment = testEnv,
                        ContentPath = Environment.CurrentDirectory
                    };

                    var app = new TestApp();
                    await app.Setup(appConfigs);
                }
                catch (Exception ex)
                {
                    testEnv.Error("Init exception " + ex.ToString());
                }
            });
        }



    }
}
