using System;
using Android.App;
using Android.OS;

namespace IRO.AndroidActivity
{
    public class ActivityEvents
    {
        /// <summary>
        /// Activity and savedInstance.
        /// </summary>
        public static event Action<Activity, Bundle> Created;

        /// <summary>
        /// Activity and savedInstance.
        /// </summary>
        public static event Action<Activity> Started;

        public static event Action<Activity, Bundle> SaveInstanceState;

        public static event Action<Activity> Resume;

        public static event Action<Activity> Stopped;

        public static event Action<Activity> Paused;

        public static event Action<Activity> Destroyed;

        /// <summary>
        /// Call once from your application OnCreate.
        /// </summary>
        /// <param name="app"></param>
        public static void Init(Application app)
        {
            app.RegisterActivityLifecycleCallbacks(new EventsActivityLifecycleCallbacks());
        }

        #region Listeners.

        internal static void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            Created?.Invoke(activity, savedInstanceState);
        }

        internal static void OnActivityStarted(Activity activity)
        {
            Started?.Invoke(activity);
        }

        internal static void OnActivityDestroyed(Activity activity)
        {
            Destroyed?.Invoke(activity);
        }

        internal static void OnActivityPaused(Activity activity)
        {
            Paused?.Invoke(activity);
        }

        internal static void OnActivityResumed(Activity activity)
        {
            Resume?.Invoke(activity);
        }

        internal static void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            SaveInstanceState?.Invoke(activity, outState);
        }

        internal static void OnActivityStopped(Activity activity)
        {
            Stopped?.Invoke(activity);
        }

        #endregion
    }
}