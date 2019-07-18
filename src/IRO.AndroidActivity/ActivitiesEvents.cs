using System;
using Android.App;
using Android.OS;

namespace IRO.AndroidActivity
{
    public class ActivitiesEvents
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

        #region Listeners.
        /// <summary>
        /// Call it from your Application class to make some functions working.
        /// </summary>
        /// <param name="activity"></param>
        public static void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            Created?.Invoke(activity, savedInstanceState);
        }

        /// <summary>
        /// Call it from your Application class to make some functions working.
        /// </summary>
        /// <param name="activity"></param>
        public static void OnActivityStarted(Activity activity)
        {
            Started?.Invoke(activity);
        }

        /// <summary>
        /// Call it from your Application class to make some functions working.
        /// </summary>
        /// <param name="activity"></param>
        public static void OnActivityDestroyed(Activity activity)
        {
            Destroyed?.Invoke(activity);
        }

        public static void OnActivityPaused(Activity activity)
        {
            Paused?.Invoke(activity);
        }

        public static void OnActivityResumed(Activity activity)
        {
            Resume?.Invoke(activity);
        }

        public static void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            SaveInstanceState?.Invoke(activity, outState);
        }

        public static void OnActivityStopped(Activity activity)
        {
            Stopped?.Invoke(activity);
        }
        #endregion
    }
}