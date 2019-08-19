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

        public async void TestLoading()
        {
            await RunXWebViewTest<TestLoading>();
        }

        public async void TestUploadsDownloads()
        {
            await RunXWebViewTest<TestUploadsDownloads>();
        }

        public async void TestJsPromiseDelay()
        {
            await RunXWebViewTest<TestJsPromiseDelay>();
        }

        public async void TestJsAwaitDelay()
        {
            await RunXWebViewTest<TestJsAwaitDelay>();
        }

        public async void TestJsAwaitError()
        {
            await RunXWebViewTest<TestJsAwaitError>();
        }

        public async void TestJsCallNative()
        {
            await RunXWebViewTest<TestJsCallNative>();
        }

        public async void TestBothCalls()
        {
            await RunXWebViewTest<TestBothCalls>();
        }

        public async void TestBothCallsSpeed()
        {
            await RunXWebViewTest<TestBothCallsSpeed>();
        }

        public async void TestTransparentView()
        {
            await RunXWebViewTest<TestTransparentView>();
        }

        public async void TestFullscreenViews()
        {
            await RunXWebViewTest<TestFullscreenViews>();
        }

        async Task RunXWebViewTest<TWebViewTest>()
            where TWebViewTest : IXWebViewTest
        {
            var test = Activator.CreateInstance<TWebViewTest>();
            try
            {
                _configs.OnTestStartedHandler?.Invoke(test);
                await test.RunTest(_provider, _testingEnvironment);
            }
            catch (Exception ex)
            {
                try
                {
                    _configs.OnTestFinishedHandler?.Invoke(test);
                }
                catch { }

                Debug.WriteLine("ERROR \n" + ex.ToString());
                _testingEnvironment.Error(ex.ToString());
            }
        }
    }
}
