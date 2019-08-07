using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using IRO.AndroidActivity;

namespace IRO.Tests.XWebView.DroidApp
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
