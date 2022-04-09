using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace PuppeteerSharp.Extensions.ChromeFinder.TestsApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
            {
                Product = Product.Chrome
            });
            await browserFetcher.DownloadAsync();

            var chromeFinder = new ChromeFinder();
            var chromePath = chromeFinder.Find();

            var chromeDataDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chromeDataDir");

            var launchOptions = new LaunchOptions
            {
                Headless = false,
                UserDataDir = chromeDataDirPath,
                //ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
                ExecutablePath = chromePath[0],
                IgnoreDefaultArgs = true,
                IgnoreHTTPSErrors = true,
                DefaultViewport = null,
                Args = new[]
                {
                    #region UI/UX part.
                  
                    "--app=https://initializing.current.application",
                    "--disable-notifications",
                    "--window-size=960,540",
                    "--compensate-for-unstable-pinch-zoom",
                    "--content-shell-hide-toolbar",
                    "--disable-dinosaur-easter-egg",
                    "--disable-device-discovery-notifications",
                    "--force-dark-mode",
                    "--mute-audio",
                    "--no-initial-navigation",
                    "--use-mobile-user-agent",
                    //"--force-tablet-mode",
                    //"--force-enable-night-mode",
                    //"--force-android-app-mode",
                    //"--force-app-mode",
                    //"--top-controls-hide-threshold=0",
                    //"--cast-app-background-color=ffffffff",
                    //"--default-background-color=ffffffff",
                    #endregion

                    #region Performance.
                    "--enable-accelerated-2d-canvas",
                    "--force-gpu-rasterization",
                    "--disable-auto-reload",
                    "--disable-crash-reporter",
                    "--disable-logging",
                    "--enable-defer-all-script-without-optimization-hints",
                    "--no-crash-upload",
                    #endregion

                    #region Security.
                    "--webview-disable-safebrowsing-support",
                    "--unlimited-storage",
                    "--allow-cross-origin-auth-prompt",
                    "--allow-external-pages",
                    "--allow-failed-policy-fetch-for-test",
                    "--allow-file-access-from-files",
                    "--allow-file-access",
                    "--allow-http-background-page",
                    "--allow-http-screen-capture",
                    "--allow-insecure-localhost",
                    "--allow-legacy-extension-manifests",
                    "--allow-loopback-in-peer-connection",
                    "--allow-no-sandbox-job",
                    "--allow-outdated-plugins",
                    "--allow-running-insecure-content",
                    "--allow-sandbox-debugging",
                    "--allow-silent-push",
                    //"--allow-third-party-modules",
                    "--allow-unsecure-dlls",
                    "--app-shell-allow-roaming",
                    "--disable-client-side-phishing-detection",
                    "--disable-site-isolation-for-policy",
                    //"--disable-web-security",
                    "--disable-windows10-custom-titlebar",
                    "--enable-experimental-extension-apis",
                    "--enable-experimental-ui-automation",
                    "--enable-experimental-webassembly-features",
                    "--enable-ftp",
                    "--enable-hung-renderer-infobar",
                    "--enable-local-file-accesses",
                    "--incognito",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    ""
                    #endregion
                },
            };
            var browser = await Puppeteer.LaunchAsync(launchOptions);
            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                browser.CloseAsync();
            };
            browser.Closed += delegate
            {
                var myProcess = Process.GetCurrentProcess();
                myProcess.CloseMainWindow();
                myProcess.Close();
                System.Environment.Exit(0);
            };

            var pages = await browser.PagesAsync();
            var page = pages[0]; ;
            await page.GoToAsync("http://www.google.com");
            //await page.ScreenshotAsync("C:\\Users\\Yura\\Desktop\\puppy.jpg");

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
