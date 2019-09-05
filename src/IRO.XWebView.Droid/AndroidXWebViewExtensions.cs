using System;
using Android.App;
using Android.Views;
using Android.Widget;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Utils;
using IRO.XWebView.Droid.Utils;

namespace IRO.XWebView.Droid
{
    public static class AndroidXWebViewExtensions
    {
        /// <summary>
        /// Extension.
        /// <para></para>
        /// Add event to android back button tap, it will invoke GoBack() method.
        /// </summary>
        /// <param name="onClose">Invoked when can't go back in browser.</param>
        public static async void UseBackButtonCrunch(
            AndroidXWebView androidXWebView, 
            View viewToRegisterEvent,
            Action onClose
            )
        {
            var backTaps = 0;
            var wantToQuitApp = 0;
            var ev = new EventHandler<View.KeyEventArgs>(async (s, e) =>
            {
                try
                {
                    if (e.KeyCode == Keycode.Back)
                    {
                        e.Handled = true;
                        if (backTaps > 0)
                        {
                            backTaps = 0;

                            //wantToQuitApp используется для двух попыток нажать назад перед оконсчательной установкой, что нельзя идти назад.
                            //Просто баг в WebView.
                            var canGoBack = androidXWebView.CanGoBack();
                            if (canGoBack)
                            {
                                wantToQuitApp = 0;
                                await androidXWebView.TryGoBack();
                            }
                            else
                            {
                                wantToQuitApp++;
                                if (wantToQuitApp == 2)
                                {
                                    await ThreadSync.Inst.TryInvokeAsync(() =>
                                    {
                                        Toast.MakeText(Application.Context, "Tap again to close.", ToastLength.Long)
                                            .Show();
                                    });
                                }
                                else if (wantToQuitApp > 2)
                                {
                                    await ThreadSync.Inst.TryInvokeAsync(() => { onClose?.Invoke(); });
                                }
                            }
                        }
                        else
                        {
                            backTaps++;
                        }
                    }
                }
                catch
                {

                }
            });
            viewToRegisterEvent.KeyPress += ev;
        }
    }
}