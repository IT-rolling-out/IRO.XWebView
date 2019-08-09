using System;
using Android.App;
using Android.Views;
using Android.Widget;
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
        public static void UseBackButtonCrunch(AndroidXWebView androidXWebView, View viewToRegisterEvent,
            Action onClose)
        {
            var backTaps = 0;
            var wantToQuitApp = 0;
            var ev = new EventHandler<View.KeyEventArgs>(async (s, e) =>
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
                            try
                            {
                                await androidXWebView.GoBack();
                            }
                            catch
                            {
                                //Ignore cancells.
                            }
                        }
                        else
                        {
                            wantToQuitApp++;
                            if (wantToQuitApp == 2)
                            {
                                ThreadSync.TryInvokeAsync(() =>
                                {
                                    Toast.MakeText(Application.Context, "Tap again to close.", ToastLength.Long)
                                        .Show();
                                });
                            }
                            else if (wantToQuitApp > 2)
                            {
                                ThreadSync.TryInvokeAsync(() => { onClose?.Invoke(); });
                            }
                        }
                    }
                    else
                    {
                        backTaps++;
                    }
                }
            });
            viewToRegisterEvent.KeyPress += ev;
        }
    }
}