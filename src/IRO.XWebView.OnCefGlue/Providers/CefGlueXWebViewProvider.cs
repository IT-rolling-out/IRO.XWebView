using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Chromely.CefGlue;
using Chromely.CefGlue.Browser;
using Chromely.Core;
using Chromely.Core.Helpers;
using Chromely.Core.Host;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.OnCefGlue.Utils;
using Xilium.CefGlue;

namespace IRO.XWebView.OnCefGlue.Providers
{
    public class CefGlueXWebViewProvider : IXWebViewProvider
    {
        Action<ChromelyConfiguration> _offScreenConfigAction;

        Action<ChromelyConfiguration> _visibleConfigAction;

        public int DefaultWidth { get; set; } = 1024;

        public int DefaultHeight { get; set; } = 768;

        public virtual async Task<IXWebView> Resolve(XWebViewVisibility prefferedVisibility = XWebViewVisibility.Hidden)
        {
            IChromelyWindow window;
            if (prefferedVisibility == XWebViewVisibility.Hidden)
            {
                window = await CreateOffScreenWindow();
            }
            else
            {
                window = await CreateVisibleWindow();
            }
            var browser = (CefGlueBrowser)window.Browser;
            return new CefGlueXWebView(browser);
        }

        public void ConfigOffScreenChromely(Action<ChromelyConfiguration> action)
        {
            _offScreenConfigAction = action;
        }

        public void ConfigVisibleChromely(Action<ChromelyConfiguration> action)
        {
            _visibleConfigAction = action;
        }

        protected virtual async Task<IChromelyWindow> CreateVisibleWindow(XWebViewVisibility prefferedVisibility = XWebViewVisibility.Hidden)
        {
            var config = ChromelyConfiguration
                .Create()
                .WithHostBounds(DefaultWidth, DefaultHeight)
                .WithHostMode(WindowState.Normal)
                .WithCustomSetting(CefSettingKeys.WindowlessRenderingEnabled, true);
            _visibleConfigAction?.Invoke(config);
            var window = await CreateAndRunWindow(config);
            return window;
        }

        protected virtual async Task<IChromelyWindow> CreateOffScreenWindow(XWebViewVisibility prefferedVisibility = XWebViewVisibility.Hidden)
        {
            var config = ChromelyConfiguration
                .Create()
                .WithHostFlag(HostFlagKey.Frameless, true)
                .WithHostBounds(DefaultWidth, DefaultHeight)
                .WithHostMode(WindowState.Normal)
                .WithCustomSetting(CefSettingKeys.WindowlessRenderingEnabled, true);
            _offScreenConfigAction?.Invoke(config);
            var window = await CreateAndRunWindow(config);

            try
            {
                if (CefRuntime.Platform == CefRuntimePlatform.Windows)
                {
                    InternalNativeMethods.ShowWindow(window.Handle, ShowWindowCommands.SW_HIDE);
                }
                else
                {
                    InternalNativeMethods.gtk_widget_hide(window.Handle);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error when try to hide CefGlue window '{ex}'.");
            }
            return window;
        }

        protected virtual async Task WaitWindowInitialized(IChromelyWindow window)
        {
            //I'm sorry, but I did not find anathor way to wait while browser initialized.
            while (window.Browser == null || window.Handle == IntPtr.Zero)
            {
                await Task.Delay(10);
            }
        }

        async Task<IChromelyWindow> CreateAndRunWindow(ChromelyConfiguration config)
        {
            var window = ChromelyWindow.Create(config);
            void DisposeWindow()
            {
                try
                {
                    window?.Close();
                    window?.Dispose();
                    CefRuntime.Shutdown();
                }
                catch { }
                window = null;
            }

            AppDomain.CurrentDomain.ProcessExit += delegate { DisposeWindow(); };
            window.RegisterServiceAssembly(Assembly.GetExecutingAssembly());
            window.ScanAssemblies();

            var windowTask = Task.Run(() =>
              {
                  try
                  {
                      window.Run(new string[0]);
                  }
                  finally
                  {
                      window.Dispose();
                  }
              });

            await WaitWindowInitialized(window);
            var browser = (CefGlueBrowser)window.Browser;
            browser.RenderProcessTerminated += delegate { DisposeWindow(); };
            return window;
        }
    }
}
