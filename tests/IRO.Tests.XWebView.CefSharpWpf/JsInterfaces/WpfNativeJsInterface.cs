namespace IRO.Tests.XWebView.CefSharpWpf.JsInterfaces
{
    public class WpfNativeJsInterface
    {
        public void UseWpfCefSharp(bool value)
        {
            TestXWebViewProvider.UseWpfProvider = value;
        }
    }
}