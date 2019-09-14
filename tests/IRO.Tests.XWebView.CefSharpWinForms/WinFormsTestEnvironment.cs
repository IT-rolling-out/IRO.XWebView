using System;
using System.Diagnostics;
using IRO.Tests.XWebView.Core;
using IRO.XWebView.Core.Utils;

namespace IRO.Tests.XWebView.CefSharpWinForms
{
    public class WinFormsTestEnvironment : ITestingEnvironment
    {
        public void Message(string str)
        {
            Debug.WriteLine(str);
        }

        public void Error(string str)
        {
            Debug.WriteLine(str);
        }
    }
}