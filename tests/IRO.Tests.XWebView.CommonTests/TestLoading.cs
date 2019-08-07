using System.Net.Mime;
using System.Threading.Tasks;
using IRO.XWebView.Core;

namespace IRO.Tests.XWebView.CommonTests
{
    public class TestLoading : IWebViewTest
    {
        public async Task RunTest(IXWebView xwv, ITestingEnvironment env)
        {
            await Task.Run(() => { });
            int delay = 2000;

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