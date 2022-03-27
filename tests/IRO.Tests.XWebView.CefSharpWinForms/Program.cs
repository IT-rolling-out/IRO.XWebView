using CefSharp;
using CefSharp.WinForms;
using IRO.Tests.XWebView.CefSharpWpf.JsInterfaces;
using IRO.Tests.XWebView.Core;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.CefSharp.WinForms.Utils;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRO.Tests.XWebView.CefSharpWinForms
{
    internal static class Program
    {
        private static Form MainForm { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            CefAssembliesResolver.ConfigureCefSharpAssembliesResolve();
            InitializeCefSharp();
            InitializeApp();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void InitializeApp()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //In example we use invisible main form as synchronization context.
            //It's important for ThreadSync that main form must be available during all app lifetime.
            MainForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false
            };
            MainForm.Load += delegate
            {
                MainForm.Size = new Size(0, 0);
                Application_Startup();
            };
            XWebViewThreadSync.Init(new WinFormsThreadSyncInvoker(MainForm));
            Application.Run(MainForm);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            var settings = new CefSettings
            {
                RemoteDebuggingPort = 9222
            };
            CefHelpers.InitializeCefIfNot(settings);
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                Cef.Shutdown();
            };
        }

        [STAThread]
        private static void Application_Startup()
        {
            Task.Run(async () =>
            {
                var provider = new TestXWebViewProvider();
                var mainXWV = await provider.Resolve(XWebViewVisibility.Visible);
                var testEnv = new WinFormsTestEnvironment(mainXWV);
                try
                {
                    mainXWV.Disposing += delegate
                    {
                        mainXWV.ThreadSync.Invoke(() =>
                        {
                            MainForm.Close();
                        });
                    };
                    mainXWV.LoadFinished += async (s, args) =>
                    {
                        try
                        {
                            ((IXWebView)s).SetPageZoomLevel(0.8);
                        }
                        catch { }
                    };

                    //Just use same object from wpf.
                    mainXWV.BindToJs(new WinFormsNativeJsInterface(), "WpfNative");

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
