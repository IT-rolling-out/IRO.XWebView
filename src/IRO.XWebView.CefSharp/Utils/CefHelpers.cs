using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Internals;
using CefOffScreen = CefSharp.OffScreen;

namespace IRO.XWebView.CefSharp.Utils
{
    public static class CefHelpers
    {
        /// <summary>
        /// Enable some features to make cef work like normal browser.
        /// </summary>
        public static void InitializeCefIfNot(
            Action<AbstractCefSettings> action = null
            )
        {
            if (Cef.IsInitialized)
                return;
            var settings = GetDefaultSettings();
            action?.Invoke(settings);
            Cef.Initialize(settings);
        }

        public static AbstractCefSettings GetDefaultSettings()
        {
            var settings = new CefOffScreen.CefSettings();
            Cef.EnableHighDPISupport();
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
            return settings;
        }

        public static async Task WaitInitialization(this CefOffScreen.ChromiumWebBrowser webBrowser)
        {
            if (webBrowser.IsBrowserInitialized)
            {
                return;
            }
            var tcs = new TaskCompletionSource<object>(TaskContinuationOptions.RunContinuationsAsynchronously);
            EventHandler ev = null;
            ev = (s, a) =>
            {
                webBrowser.BrowserInitialized -= ev;
                tcs?.TrySetResult(null);
                tcs = null;
            };
            webBrowser.BrowserInitialized += ev;
            if (!webBrowser.IsBrowserInitialized)
            {
                await tcs.Task;
            }
        }
    }
}
