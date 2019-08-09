using System;
using Android.App;
using Android.Runtime;
using IRO.AndroidActivity;
using Plugin.CurrentActivity;

namespace IRO.Tests.XWebView.DroidApp
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CrossCurrentActivity.Current.Init(this);
            ActivityEvents.Init(this);
            //A great place to initialize Xamarin.Insights and Dependency Services!
        }
    }
}