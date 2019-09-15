using System;
using System.Diagnostics;
using System.Windows.Forms;
using IRO.Tests.XWebView.Core;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Utils;
using IRO.XWebView.Extensions;

namespace IRO.Tests.XWebView.CefSharpWinForms
{
    public class WinFormsTestEnvironment : ITestingEnvironment
    {
        readonly IXWebView _mainXWV;

        public WinFormsTestEnvironment(IXWebView mainXWV)
        {
            _mainXWV = mainXWV ?? throw new ArgumentNullException(nameof(mainXWV));
        }

        public void Message(string str)
        {
             Debug.WriteLine(str);
             _mainXWV.ShowToast(str);
           
        }

        public void Error(string str)
        {
            Debug.WriteLine(str);
            ThreadSync.Inst.Invoke(() =>
            {
                MessageBox.Show("ERROR  " + str); 
            });
        }
    }
}