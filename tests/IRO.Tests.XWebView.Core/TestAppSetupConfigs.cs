using System;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core
{
    public struct TestAppSetupConfigs
    {
        public string ContentPath { get; set; }

        public IXWebView MainXWebView { get; set; }

        public IXWebViewProvider Provider{ get; set; }

        public ITestingEnvironment TestingEnvironment { get; set; }

        public Action<IXWebViewTest> OnTestStartedHandler { get; set; }

        public Action<IXWebViewTest> OnTestFinishedHandler { get; set; }
    }
}