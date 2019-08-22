﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using IRO.Tests.XWebView.CefSharpWpf.JsInterfaces;
using IRO.Tests.XWebView.Core;
using IRO.XWebView.CefSharp;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.CefSharp.Wpf;
using IRO.XWebView.CefSharp.Wpf.Providers;
using IRO.XWebView.CefSharp.Wpf.Utils;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core;

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
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                Cef.Shutdown();
            };
            //!Copied AnyCpu CefSharp example, but it throw exception. 
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            InitializeCefSharp();
            //It seems it's because of netcore 3.

        }

        [STAThread]
        void Application_Startup(object sender, StartupEventArgs e)
        {
            Task.Run(async () =>
            {
                var testEnv = new WpfTestEnvironment();
                try
                {
                    var wpfProvider = new WpfCefSharpXWebViewProvider();
                    var chromiumWindow = wpfProvider.CreateWpfWindow();
                    WpfThreadSync.Invoke(() =>
                    {
                        chromiumWindow.Show(); 
                    });
                    chromiumWindow.SetVisibilityState(XWebViewVisibility.Visible);
                    var mainXWV = await CefSharpXWebView.Create(chromiumWindow);
                    mainXWV.BindToJs(new WpfNativeJsInterface(), "WpfNative");
                    var provider = new TestXWebViewProvider();

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
