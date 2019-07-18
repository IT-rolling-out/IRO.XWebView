using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IRO.AndroidActivity;

namespace IRO.ImprovedWebView.Droid
{
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            //A great place to initialize Xamarin.Insights and Dependency Services!
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            ActivitiesEvents.OnActivityCreated(activity, savedInstanceState);
        }

        public void OnActivityDestroyed(Activity activity)
        {
            ActivitiesEvents.OnActivityDestroyed(activity);
        }

        public void OnActivityPaused(Activity activity)
        {
            ActivitiesEvents.OnActivityPaused(activity);
        }

        public void OnActivityResumed(Activity activity)
        {
            ActivitiesEvents.OnActivityResumed(activity);
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            ActivitiesEvents.OnActivitySaveInstanceState(activity,outState);
        }

        public void OnActivityStarted(Activity activity)
        {
            ActivitiesEvents.OnActivityStarted(activity);
        }

        public void OnActivityStopped(Activity activity)
        {
            ActivitiesEvents.OnActivityStopped(activity);
        }
    }
}
}