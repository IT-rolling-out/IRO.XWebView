using System.Net.Mime;
using System.Threading.Tasks;
using IRO.ImprovedWebView.Core;

namespace IRO.Tests.ImprovedWebView.CommonTests
{
    public class TestLoading : IWebViewTest
    {
        public async Task RunTest(IImprovedWebView iwv, ITestingEnvironment env)
        {
            await Task.Run(() => { });
            int delay = 2000;

            //Choose websites that can load long time.
            //This three must be aborted in test.
            iwv.TryLoadUrl("https://stackoverflow.com");
            iwv.TryLoadUrl("https://twitter.com");
            iwv.TryLoadUrl("https://visualstudio.microsoft.com/ru/");

            await Task.Delay(50);
            env.Message($"3 loads aborted.");

            var loadRes = await iwv.LoadUrl("https://www.microsoft.com/");
            env.Message($"Loaded {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await iwv.Reload();
            env.Message($"Reloaded {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await iwv.LoadUrl("https://www.youtube.com/");
            env.Message($"Loaded {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await iwv.LoadUrl("https://www.google.com/");
            env.Message($"Loaded {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await iwv.GoBack();
            env.Message($"GoBack {loadRes.Url}");
            await Task.Delay(delay);

            loadRes = await iwv.GoForward();
            env.Message($"GoForward {loadRes.Url}");
            await Task.Delay(delay);

        }
    }
}