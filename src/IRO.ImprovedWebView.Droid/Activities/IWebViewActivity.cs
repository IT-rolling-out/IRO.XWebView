using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Droid.EventsProxy;

namespace IRO.ImprovedWebView.Droid.Activities
{
    public interface IWebViewActivity
    {
        void ToggleVisibilityState(ImprovedWebViewVisibility visibility);

        event Action<IWebViewActivity> Finishing;

        WebView CurrentWebView { get; }

        Task WaitWebViewInitialized();

        Task WebViewWrapped(AndroidImprovedWebView improvedWebView);

        void Finish();
    }
}