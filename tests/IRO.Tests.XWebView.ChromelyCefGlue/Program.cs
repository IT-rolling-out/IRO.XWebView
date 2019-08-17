using System;
using System.Threading.Tasks;
using Chromely.CefGlue.Winapi;
using IRO.Tests.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.OnCefGlue;
using IRO.XWebView.OnCefGlue.Providers;
using Xilium.CefGlue;

namespace IRO.Tests.XWebView.ChromelyCefGlue
{
    class Program
    {
        static void Main(string[] args)
        {
            //NOTE: You can hide console, gide here https://github.com/chromelyapps/Chromely/wiki/Getting-Started-CefGlue-Winapi-(.NET-Core) .
            HostHelpers.SetupDefaultExceptionHandlers();

            Task.Run(async () =>
            {
                var testEnv = new ConsoleTestingEnvironment();
                try
                {
                    var provider = new CefGlueXWebViewProvider();
                    var mainXWV = (CefGlueXWebView)await provider.Resolve(XWebViewVisibility.Visible);

                    var appConfigs = new TestAppSetupConfigs
                    {
                        MainXWebView = mainXWV,
                        Provider = provider,
                        TestingEnvironment = testEnv,
                        ContentPath = Environment.CurrentDirectory
                    };

                    var br = mainXWV.CefGlueBrowser.CefBrowser;
                    while (true)
                    {
                        br.ExecuteJavascript();
                        await Task.Delay(1500);
                    }
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
    }
}
