using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Webkit;
using IRO.AndroidActivity;
using Java.Lang;
using TheFinestArtist.FinestWebViewLib;
using TheFinestArtist.FinestWebViewLib.Listeners;
using Exception = System.Exception;

namespace IRO.XWebView.Droid.OnFinestWebView
{
    public class OverriddenFinestWebViewBuilder<TActivityToShow> : FinestWebView.Builder
        where TActivityToShow : OverriddenFinestWebViewActivity
    {
        readonly TaskCompletionSource<object> _waitWebViewInitialized_TaskCompletionSource =
            new TaskCompletionSource<object>();

        protected OverriddenFinestWebViewBuilder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference,
            transfer)
        {
        }

        public OverriddenFinestWebViewBuilder(Activity activity) : base(activity)
        {
        }

        public OverriddenFinestWebViewBuilder(Context context) : base(context)
        {
        }

        public TActivityToShow CurrentActivity { get; private set; }

        protected override void Show(string url, string data)
        {
            if (CurrentActivity != null)
            {
                throw new Exception(
                    $"You must use new {GetType().Name} every time when you create activity."
                );
            }

            Url = url;
            Data = data;
            Key = new Integer(JavaSystem.IdentityHashCode(this));
            var newList = new List<WebViewListener>();
            foreach (var item in Listeners)
            {
                newList.Add((WebViewListener)item);
            }

            if (Listeners.Count != 0)
            {
                new BroadCastManager(Context, Key.IntValue(), newList);
            }

            var intent = new Intent(Context, typeof(TActivityToShow));
            intent.PutExtra("builder", this);
            ActivityExtensions.StartNewActivity(intent).ContinueWith((t) =>
            {
                CurrentActivity = (TActivityToShow)t.Result;
                _waitWebViewInitialized_TaskCompletionSource.SetResult(null);
            });




            if (Context is Activity ownerActivity)
            {
                ownerActivity.OverridePendingTransition(
                    AnimationOpenEnter.IntValue(),
                    AnimationOpenExit.IntValue()
                );
            }
        }

        /// <summary>
        /// Use <see cref="CurrentActivity"/> or <see cref="CurrentWebView"/>
        /// only after awaiting.
        /// </summary>
        /// <returns></returns>
        public async Task WaitWebViewShowed()
        {
            await _waitWebViewInitialized_TaskCompletionSource.Task;
        }
    }
}