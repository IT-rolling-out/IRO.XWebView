using CefSharp;
using IRO.XWebView.Core.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IRO.XWebView.CefSharp.Utils
{

    public static class CefHelpers
    {
        /// <summary>
        /// Enable some features to make cef work like normal browser.
        /// </summary>
        public static void InitializeCefIfNot(CefSettingsBase settings)
        {
            if (Cef.IsInitialized)
                return;
            AddDefaultSettings(settings);
            CefSharpSettings.ConcurrentTaskExecution = true;
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            CefSharpSettings.ShutdownOnExit = true;
            Cef.Initialize(settings);

        }

        public static void AddDefaultSettings(CefSettingsBase settings)
        {
            Cef.EnableHighDPISupport();           
            settings.WindowlessRenderingEnabled = true;
            settings.MultiThreadedMessageLoop = true;
            settings.PersistUserPreferences = true;
            settings.CefCommandLineArgs.Add("high-dpi-support", "1");
            settings.CefCommandLineArgs.Add("force-device-scale-factor", "1");
            settings.PersistSessionCookies = true;
            settings.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";
            var cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chromeCahce");
            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);
            settings.CachePath = cachePath;
        }

        public static async Task WaitInitialization(this IWebBrowser webBrowser)
        {
            var isBrowserInitialized = await XWebViewThreadSync.Inst.InvokeAsync(() => webBrowser.IsBrowserInitialized);
            while (!isBrowserInitialized)
            {
                await Task.Delay(10).ConfigureAwait(false);
                isBrowserInitialized = await XWebViewThreadSync.Inst.InvokeAsync(() => webBrowser.IsBrowserInitialized);
            }
        }
    }
}
