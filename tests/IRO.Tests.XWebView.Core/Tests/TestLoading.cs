using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestLoading : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env)
        {
            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Visible);
            await Task.Run(() => { });
            var delay = 2000;

            //Choose websites that can load long time.
            //This three must be aborted in test.
            xwv.TryLoadUrl("https://stackoverflow.com");
            xwv.TryLoadUrl("https://twitter.com");
            xwv.TryLoadUrl("https://visualstudio.microsoft.com/ru/");

            await Task.Delay(50);
            env.Message($"3 loads aborted.");

            var loadRes = await xwv.LoadUrl("https://www.microsoft.com/");
            env.Message($"Loaded {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await xwv.Reload();
            env.Message($"Reloaded {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await xwv.LoadUrl("https://www.youtube.com/");
            env.Message($"Loaded {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await xwv.LoadUrl("https://www.google.com/");
            env.Message($"Loaded {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await xwv.GoBack();
            env.Message($"GoBack {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await xwv.GoForward();
            env.Message($"GoForward {loadRes.Url}");
            await Task.Delay(delay);
        }
    }
}