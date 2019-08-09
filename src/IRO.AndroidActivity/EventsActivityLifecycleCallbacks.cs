using Android.App;
using Android.OS;
using Android.Runtime;
using Java.Lang;

namespace IRO.AndroidActivity
{
    [Preserve(AllMembers = true)]
    class EventsActivityLifecycleCallbacks : Object, Application.IActivityLifecycleCallbacks
    {
        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            ActivityEvents.OnActivityCreated(activity, savedInstanceState);
        }

        public void OnActivityDestroyed(Activity activity)
        {
            ActivityEvents.OnActivityDestroyed(activity);
        }

        public void OnActivityPaused(Activity activity)
        {
            ActivityEvents.OnActivityPaused(activity);
        }

        public void OnActivityResumed(Activity activity)
        {
            ActivityEvents.OnActivityResumed(activity);
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            ActivityEvents.OnActivitySaveInstanceState(activity, outState);
        }

        public void OnActivityStarted(Activity activity)
        {
            ActivityEvents.OnActivityStarted(activity);
        }

        public void OnActivityStopped(Activity activity)
        {
            ActivityEvents.OnActivityStopped(activity);
        }
    }
}