using System;
using System.Diagnostics;
using System.Windows;
using IRO.Tests.XWebView.Core;
using IRO.XWebView.CefSharp.Wpf.Utils;
using IRO.XWebView.Core.Utils;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace IRO.Tests.XWebView.CefSharpWpf
{
    public class WpfTestEnvironment : ITestingEnvironment
    {
        private Notifier _notifier;
        private int _prevWidth;

        public void Message(string str)
        {
            XWebViewThreadSync.Inst.Invoke(() =>
            {
                RecreateNotifierIfNeed();
                _notifier.ShowInformation(str);
            });
        }

        public void Error(string str)
        {
            Debug.WriteLine(str);
            MessageBox.Show(str);
        }

        void RecreateNotifierIfNeed()
        {
            if (_prevWidth == (int)Application.Current.MainWindow.Width)
            {
                return;
            }
            _notifier?.Dispose();
            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(5),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(10));
                cfg.DisplayOptions.Width = Application.Current.MainWindow.Width * 0.9;

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
            _prevWidth = (int)Application.Current.MainWindow.Width;
        }
    }
}