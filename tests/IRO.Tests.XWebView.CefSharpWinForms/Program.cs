using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using IRO.Tests.XWebView.CefSharpWpf.JsInterfaces;
using IRO.Tests.XWebView.Core;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.CefSharp.WinForms.Utils;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;
using IRO.XWebView.Extensions;

namespace IRO.Tests.XWebView.CefSharpWinForms
{
    static class Program
    {
        static Form MainForm { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            InitializeCefSharp();
            //In example we use invisible main form as synchronization context.
            //It's important for ThreadSync that main form must be available during all app lifetime.
            MainForm = new Form();
            MainForm.FormBorderStyle = FormBorderStyle.None;
            MainForm.ShowInTaskbar = false;
            MainForm.Load += delegate
            {
                MainForm.Size = new Size(0, 0);
                Application_Startup();
            };
            XWebViewThreadSync.Init(new WinFormsThreadSyncInvoker(MainForm));
            Application.Run(MainForm);

        }

        [STAThread]
        static void Application_Startup()
        {
            Task.Run(async () =>
            {
                var provider = new TestXWebViewProvider();
                var mainXWV = await provider.Resolve(XWebViewVisibility.Visible);
                var testEnv = new WinFormsTestEnvironment(mainXWV);
                try
                {
                    mainXWV.Disposing += delegate {
                        mainXWV.ThreadSync.Invoke(() =>
                        {
                            MainForm.Close();
                        });
                    };
                    mainXWV.LoadFinished += (s, args) =>
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
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

        // Will attempt to load missing assembly from either x86 or x64 subdir
        // Required by CefSharp to load the unmanaged dependencies when running using AnyCPU
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
