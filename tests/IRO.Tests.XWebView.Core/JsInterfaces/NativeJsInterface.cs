using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using IRO.Tests.XWebView.Core.Tests;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.JsInterfaces
{
    /// <summary>
    /// Object that passed to webview.
    /// </summary>
    public class NativeJsInterface
    {
        readonly IXWebView _mainXWebView;

        readonly IXWebViewProvider _provider;

        readonly ITestingEnvironment _testingEnvironment;

        readonly TestAppSetupConfigs _configs;

        public NativeJsInterface(TestAppSetupConfigs conf)
        {
            _configs = conf;
            _mainXWebView = conf.MainXWebView;
            _provider = conf.Provider ?? throw new NullReferenceException(nameof(conf.Provider));
            _testingEnvironment = conf.TestingEnvironment ?? throw new NullReferenceException(nameof(conf.TestingEnvironment));
        }

        public string GetXWebViewName()
        {
            return _mainXWebView.BrowserName;
        }

        public void TestLoading()
        {
            RunXWebViewTest<TestLoading>();
        }

        public void TestUploadsDownloads()
        {
            RunXWebViewTest<TestUploadsDownloads>();
        }

        public void TestJsPromiseDelay()
        {
            RunXWebViewTest<TestJsPromiseDelay>();
        }

        public void TestJsAwaitDelay()
        {
            RunXWebViewTest<TestJsAwaitDelay>();
        }

        public void TestJsAwaitError()
        {
            RunXWebViewTest<TestJsAwaitError>();
        }

        public void TestJsCallNative()
        {
            RunXWebViewTest<TestJsCallNative>();
        }

        public void TestBothCalls()
        {
            RunXWebViewTest<TestBothCalls>();
        }

        public void TestBothCallsSpeed()
        {
            RunXWebViewTest<TestBothCallsSpeed>();
        }

        public void TestTransparentView()
        {
            RunXWebViewTest<TestTransparentView>();
        }

        public void TestFullscreenViews()
        {
            RunXWebViewTest<TestFullscreenViews>();
        }

        public void TestJQueryInclude()
        {
            RunXWebViewTest<TestJQueryInclude>();
        }

        public void TestScreenshotViaJs()
        {
            RunXWebViewTest<TestScreenshotViaJs>();
        }

        public void TestToast()
        {
            RunXWebViewTest<TestToast>();
        }

        public void TestGetHtmlViaJs()
        {
             RunXWebViewTest<TestGetHtmlViaJs>();
        }

        void RunXWebViewTest<TWebViewTest>()
            where TWebViewTest : IXWebViewTest
        {

            Task.Run(async () =>
            {
                var test = Activator.CreateInstance<TWebViewTest>();
                try
                {
                    _configs.OnTestStartedHandler?.Invoke(test);
                    await test.RunTest(_provider, _testingEnvironment, _configs);
                }
                catch (Exception ex)
                {
                    try
                    {
                        _configs.OnTestFinishedHandler?.Invoke(test);
                    }
                    catch
                    {
                    }

                    Debug.WriteLine("ERROR \n" + ex.ToString());
                    _testingEnvironment.Error(ex.ToString());
                }
            });
        }
    }
}
