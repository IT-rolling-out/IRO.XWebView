namespace IRO.Tests.XWebView.CefSharpWpf.JsInterfaces
{
    public class WpfNativeJsInterface
    {
        public void UseOffscreenCefSharpChanged(bool value)
        {
            TestXWebViewProvider.UseOffScreenProvider = value;
        }
    }
}