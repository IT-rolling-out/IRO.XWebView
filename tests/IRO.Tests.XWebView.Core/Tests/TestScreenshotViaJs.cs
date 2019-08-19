using System.IO;
using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Extensions;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestScreenshotViaJs : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env, TestAppSetupConfigs appConfigs)
        {
            var url = "http://info.cern.ch/hypertext/WWW/TheProject.html";
            env.Message($"Will load {url} in background and make screenshot and open image in new tab.");
            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Hidden);
            await xwv.LoadUrl(url);
            await Task.Delay(1000);
            await xwv.AttachBridge();
            //If you will catch 'unsafe-eval' exception it means that your browser not configured correctly.
            var base64img = await xwv.ScreenshotViaJs("body");
            xwv.Dispose();
            var xwvVisible = await xwvProvider.Resolve(XWebViewVisibility.Visible);
            env.Message($"Hidden XWebView disposed after screenshot.");
            try
            {
                var bitmap = Htm2CanvasExtensions.Base64StringToBitmap(base64img);
                var imageFilePath = appConfigs.ContentPath + "/screenshot.png";
                if (File.Exists(imageFilePath))
                    File.Delete(imageFilePath);
                bitmap.Save(imageFilePath);
                env.Message($"Image saved and will be loaded.");
                await xwvVisible.LoadUrl("file://" + imageFilePath);
            }
            catch
            {
                //If Image.FromStream not supported on platform.
                await xwvVisible.LoadUrl(base64img);
            }
        }
    }
}