using System;
using System.Windows;
using IRO.Tests.XWebView.Core;
using IRO.XWebView.CefSharp.Wpf.Utils;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace IRO.Tests.XWebView.CefSharpWpf
{
    public class WpfTestEnvironment : ITestingEnvironment
    {
        public void Message(string str)
        {
            ThreadSync.Inst.Invoke(() =>
            {
                var notifier = new Notifier(cfg =>
                {
                    cfg.PositionProvider = new WindowPositionProvider(
                        parentWindow: Application.Current.MainWindow,
                        corner: Corner.TopRight,
                        offsetX: 10,
                        offsetY: 10);

                    cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                        notificationLifetime: TimeSpan.FromSeconds(7),
                        maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                    cfg.Dispatcher = Application.Current.Dispatcher;
                });
                notifier.ShowInformation(str);
                notifier.Dispose();
            });
        }

        public void Error(string str)
        {
            MessageBox.Show(str);
        }
    }
}