using IRO.Tests.XWebView.CefSharpWinForms;

namespace IRO.Tests.XWebView.CefSharpWpf.JsInterfaces
{
    public class WinFormsNativeJsInterface
    {
        public void UseOffscreenCefSharpChanged(bool value)
        {
            TestXWebViewProvider.UseOffScreenProvider = value;
        }
    }
}