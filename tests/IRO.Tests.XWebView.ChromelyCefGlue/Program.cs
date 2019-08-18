using System;
using System.Threading.Tasks;
using Chromely.Core;
using Chromely.Core.Helpers;
using Chromely.Core.Infrastructure;
using IRO.Tests.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.OnCefGlue;
using IRO.XWebView.OnCefGlue.Providers;
using Xilium.CefGlue;

namespace IRO.Tests.XWebView.ChromelyCefGlue
{
    class Program
    {
        static int _cefRemoteDebugPort = 9222;

        static void Main(string[] args)
        {
            //NOTE: You can hide console, gide here https://github.com/chromelyapps/Chromely/wiki/Getting-Started-CefGlue-Winapi-(.NET-Core) .
            //HostHelpers.SetupDefaultExceptionHandlers();

            Task.Run(async () =>
            {
                var testEnv = new ConsoleTestingEnvironment();
                try
                {
                    var provider = new CefGlueXWebViewProvider();
                    provider.ConfigVisibleChromely(AddConfigs);
                    provider.ConfigOffScreenChromely(AddConfigs);
                    var mainXWV = (CefGlueXWebView)await provider.Resolve(XWebViewVisibility.Visible);

                    var appConfigs = new TestAppSetupConfigs
                    {
                        MainXWebView = mainXWV,
                        Provider = provider,
                        TestingEnvironment = testEnv,
                        ContentPath = Environment.CurrentDirectory
                    };
                    var br = mainXWV.CefGlueBrowser.CefBrowser;
                    var app = new TestApp();
                    await app.Setup(appConfigs);
                }
                catch (Exception ex)
                {
                    testEnv.Error("Init exception " + ex);
                    throw;
                }
            });

            while (true)
            {
                Console.ReadLine();
            }
        }

        static void AddConfigs(ChromelyConfiguration conf)
        {
            conf.DebuggingMode = true;
            conf.WithCustomSetting(CefSettingKeys.RemoteDebuggingPort, _cefRemoteDebugPort++)
                .WithGtkHostApi()
                .WithCustomSetting(CefSettingKeys.UserDataPath, Environment.CurrentDirectory+"/user_data")
                .WithLogFile("logs\\chromely.cef_new.log")
                .WithLogSeverity(LogSeverity.Info)
                .UseDefaultLogger("logs\\chromely_new.log");
        }
    }
}
