using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Extensions;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestScreenshotViaJs : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
            var url = "http://info.cern.ch/hypertext/WWW/TheProject.html";
            ShowMessage($"Will load {url} in background and make screenshot and open image in new tab.");
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Hidden);
            await xwv.LoadUrl(url);
            await Task.Delay(1000);
            await xwv.AttachBridge();
            //If you will catch 'unsafe-eval' exception it means that your browser not configured correctly.
            var base64img = await xwv.ScreenshotViaJs("body");
            xwv.Dispose();
            ShowMessage($"Hidden XWebView disposed after screenshot.");
            try
            {
                var bitmap = Htm2CanvasExtensions.Base64StringToBitmap(base64img);
                var imageFilePath = AppConfigs.ContentPath + "/screenshot.png";
                if (File.Exists(imageFilePath))
                    File.Delete(imageFilePath);
                bitmap.Save(imageFilePath);
                ShowMessage($"Image saved and will be loaded.");
                Process.Start(imageFilePath);
            }
            catch
            {
                var xwvVisible = await XWVProvider.Resolve(XWebViewVisibility.Visible);
                //If Image.FromStream not supported on platform.
                await xwvVisible.LoadUrl(base64img);
            }
        }
    }
}