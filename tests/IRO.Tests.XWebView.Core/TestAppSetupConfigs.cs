using System;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core
{
    public struct TestAppSetupConfigs
    {
        public string ContentPath { get; set; }

        public IXWebView MainXWebView { get; set; }

        public IXWebViewProvider Provider { get; set; }

        public ITestingEnvironment TestingEnvironment { get; set; }

        public Action<BaseXWebViewTest> OnTestStartedHandler1 { get; set; }

        public Action<BaseXWebViewTest> OnTestFinishedHandler1 { get; set; }
    }
}