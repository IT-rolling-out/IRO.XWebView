using System;
using Android.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using IRO.ImprovedWebView.Droid.Common;

namespace IRO.ImprovedWebView.Droid
{
    public static class AndroidImprovedWebViewExtensions
    {
        /// <summary>
        /// Extension.
        /// <para></para>
        /// Add event to android back button tap, it will invoke GoBack() method.
        /// </summary>
        /// <param name="onClose">Invoked when can't go back in browser.</param>
        public static void UseBackButtonCrunch(AndroidImprovedWebView androidImprovedWebView, View viewToRegisterEvent, Action onClose)
        {
            int backTaps = 0;
            int wantToQuitApp = 0;
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
                        var canGoBack = androidImprovedWebView.CanGoBack();
                        if (canGoBack)
                        {
                            wantToQuitApp = 0;
                            try
                            {
                                await androidImprovedWebView.GoBack();
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
                                    Toast.MakeText(Application.Context, "Tap again to close.", ToastLength.Long).Show();
                                });
                            }
                            else if (wantToQuitApp > 2)
                            {
                                ThreadSync.TryInvokeAsync(() =>
                                {
                                    onClose?.Invoke();
                                });
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