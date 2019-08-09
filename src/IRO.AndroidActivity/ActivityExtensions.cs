using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;

namespace IRO.AndroidActivity
{
    public static class ActivityExtensions
    {
        static readonly Random random = new Random();

        /// <summary>
        /// You doesn`t need to check requestCode in result, it will be checked automatically. 
        /// If requestCode don`t match, task will pe finished with exception.
        /// </summary>
        public static Task<ActivityResultArgs> StartActivityAndReturnResult(Intent intent, int requestCode)
        {
            var taskCompletionSource =
                new TaskCompletionSource<ActivityResultArgs>(TaskCreationOptions.RunContinuationsAsynchronously);

            ActivityResultReturnedDelegate evHandler = null;
            evHandler = (resultArgs) =>
            {
                //Returned result event from activity. 
                //Normal is resultArgs.RequestCode == requestCode.
                ReceiveResultTransperedActivity.ActivityResultReturned -= evHandler;
                if (resultArgs.RequestCode == requestCode)
                {
                    taskCompletionSource.SetResult(resultArgs);
                }
                else
                {
                    taskCompletionSource.SetException(
                        new Exception("RequestCode in activity result doesn`t match to passed RequestCode."));
                }
            };


            //Set current params.
            var hiddenActivityStartIntent = new Intent(Application.Context, typeof(ReceiveResultTransperedActivity));
            //hiddenActivityStartIntent.PutExtra(ReceiveResultTransperedActivity.IncludedIntentParamName, intent);
            //hiddenActivityStartIntent.PutExtra(ReceiveResultTransperedActivity.RequestCodeParamName, requestCode);
            ReceiveResultTransperedActivity.CurrentIntent = intent;
            ReceiveResultTransperedActivity.CurrentRequestCode = requestCode;
            ReceiveResultTransperedActivity.ActivityResultReturned += evHandler;
            hiddenActivityStartIntent.AddFlags(ActivityFlags.NewTask);

            //Create an intermediate transparent activity in the context of the application(not in the context of another activation).
            Application.Context.StartActivity(hiddenActivityStartIntent);
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// RequestCode will be generated. 
        /// You doesn`t need to check requestCode in result, it will be checked automatically. 
        /// If requestCode don`t match, task will pe finished with exception.
        /// </summary>
        public static Task<ActivityResultArgs> StartActivityAndReturnResult(Intent intent)
        {
            return StartActivityAndReturnResult(intent, random.Next(100000, 999999));
        }

        public static async Task<Activity> StartNewActivity(Type activityType)
        {
            var intent = new Intent(Application.Context, activityType);
            const string UniqCreateIdentifierKey = "UniqCreateIdentifier";
            var randomKey = random.Next(100000, 999999);
            intent.PutExtra(UniqCreateIdentifierKey, randomKey);
            intent.AddFlags(ActivityFlags.NewTask);
            var tcs = new TaskCompletionSource<Activity>(TaskCreationOptions.RunContinuationsAsynchronously);
            Action<Activity> startedHandler = null;
            startedHandler = (activity) =>
            {
                try
                {
                    var resolvedRandomKey = activity.Intent.GetIntExtra(UniqCreateIdentifierKey, 0);
                    if (randomKey == resolvedRandomKey)
                    {
                        ActivityEvents.Started -= startedHandler;
                        tcs.SetResult(activity);
                    }
                }
                catch
                {
                }
            };
            ActivityEvents.Started += startedHandler;
            Application.Context.StartActivity(intent);
            return await tcs.Task;
        }

        public static async Task<TActivity> StartNewActivity<TActivity>()
            where TActivity : Activity
        {
            return (TActivity) await StartNewActivity(typeof(TActivity));
        }
    }
}