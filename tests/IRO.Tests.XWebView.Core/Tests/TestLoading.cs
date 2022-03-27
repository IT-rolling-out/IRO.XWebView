using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestLoading : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
            //Choose websites that can load long time.
            //This three must be aborted in test.
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Visible);
            var delay = 2000;

            var loadRes = await xwv.LoadUrl("https://www.google.com/");
            ShowMessage($"Loaded {loadRes.Url}");
            await Task.Delay(delay);

            xwv.TryLoadUrl("https://stackoverflow.com");
            xwv.TryLoadUrl("https://twitter.com");
            xwv.TryLoadUrl("https://visualstudio.microsoft.com/ru/");

            await Task.Delay(50);
            ShowMessage($"3 loads aborted.");

            loadRes = await xwv.LoadUrl("https://www.microsoft.com/");
            ShowMessage($"Loaded {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await xwv.Reload();
            ShowMessage($"Reloaded {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await xwv.LoadUrl("https://www.youtube.com/");
            ShowMessage($"Loaded {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await xwv.LoadUrl("https://github.com/IT-rolling-out");
            ShowMessage($"Loaded {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await xwv.GoBack();
            ShowMessage($"GoBack {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await xwv.GoForward();
            ShowMessage($"GoForward {loadRes.Url}");
            await Task.Delay(delay);
        }
    }
}