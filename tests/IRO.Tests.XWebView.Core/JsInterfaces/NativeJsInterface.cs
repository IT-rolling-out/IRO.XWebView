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
    public class TestsMainMenuJsInterface
    {
        readonly IXWebView _mainXWebView;

        readonly IXWebViewProvider _provider;

        readonly ITestingEnvironment _testingEnvironment;

        public TestsMainMenuJsInterface(IXWebView mainXWebView, IXWebViewProvider provider, ITestingEnvironment testingEnvironment)
        {
            _mainXWebView = mainXWebView;
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _testingEnvironment = testingEnvironment ?? throw new ArgumentNullException(nameof(testingEnvironment));
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
                await test.RunTest(_provider, _testingEnvironment);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR \n" + ex.ToString());
                _testingEnvironment.Error(ex.ToString());
            }
        }
    }
}
