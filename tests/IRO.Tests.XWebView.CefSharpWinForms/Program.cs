using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using IRO.Tests.XWebView.CefSharpWpf.JsInterfaces;
using IRO.Tests.XWebView.Core;
using IRO.XWebView.CefSharp;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.CefSharp.WinForms;
using IRO.XWebView.CefSharp.WinForms.Utils;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Utils;
using IRO.XWebView.Extensions;
using IXWebView = IRO.XWebView.Core.IXWebView;

namespace IRO.Tests.XWebView.CefSharpWinForms
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ThreadSync.Init(new WinFormsThreadSyncInvoker());
            InitializeCefSharp();
            var form = new CefSharpXWebViewForm();
            var mainXWV = new CefSharpXWebView(form);
            form.SetVisibilityState(XWebViewVisibility.Visible);
            var testEnv = new WinFormsTestEnvironment();

            form.Load += async delegate
            {
                try
                {
                    await mainXWV.WaitInitialization();
                    var provider = new TestXWebViewProvider();
                    mainXWV.LoadFinished += (s, args) =>
                    {
                        try
                        {
                            ((IXWebView)s).SetZoomLevel(0.8);
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
            };

            try
            {
                Application.Run(form);
            }
            catch (Exception ex)
            {
                testEnv.Error("Init exception " + ex.ToString());
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            var settings = new CefSettings();
            CefHelpers.AddDefaultSettings(settings);
            settings.RemoteDebuggingPort = 9222;
            Cef.Initialize(settings, false, browserProcessHandler: null);
        }
    }
}
