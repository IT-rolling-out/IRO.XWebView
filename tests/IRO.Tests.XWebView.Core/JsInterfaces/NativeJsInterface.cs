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

        public async Task TestLoading()
        {
            await RunXWebViewTest<TestLoading>();
        }

        public async Task TestUploadsDownloads()
        {
            await RunXWebViewTest<TestUploadsDownloads>();
        }

        public async Task TestJsPromiseDelay()
        {
            await RunXWebViewTest<TestJsPromiseDelay>();
        }

        public async Task TestJsAwaitDelay()
        {
            await RunXWebViewTest<TestJsAwaitDelay>();
        }

        public async Task TestJsAwaitError()
        {
            await RunXWebViewTest<TestJsAwaitError>();
        }

        public async Task TestJsCallNative()
        {
            await RunXWebViewTest<TestJsCallNative>();
        }

        public async Task TestBothCalls()
        {
            await RunXWebViewTest<TestBothCalls>();
        }

        public async Task TestBothCallsSpeed()
        {
            await RunXWebViewTest<TestBothCallsSpeed>();
        }

        public async Task TestTransparentView()
        {
            await RunXWebViewTest<TestTransparentView>();
        }

        public async Task TestFullscreenViews()
        {
            await RunXWebViewTest<TestFullscreenViews>();
        }

        public async Task TestJQueryInclude()
        {
            await RunXWebViewTest<TestJQueryInclude>();
        }

        public async Task TestScreenshotViaJs()
        {
            await RunXWebViewTest<TestScreenshotViaJs>();
        }

        async Task RunXWebViewTest<TWebViewTest>()
            where TWebViewTest : IXWebViewTest
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
                catch { }

                Debug.WriteLine("ERROR \n" + ex.ToString());
                _testingEnvironment.Error(ex.ToString());
            }
        }
    }
}
