using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace IRO.AndroidActivity
{
    [Activity(Theme = "@android:style/Theme.Translucent.NoTitleBar", LaunchMode = LaunchMode.Multiple)]
    class ReceiveResultTransperedActivity : Activity
    {
        public static Intent CurrentIntent { get; set; }

        public static int CurrentRequestCode { get; set; }
        //public const string IncludedIntentParamName = "IncludedIntentParam";
        //public const string RequestCodeParamName = "IncludedIntentParam";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //var intent = (Intent)this.Intent.GetParcelableExtra(IncludedIntentParamName);
            //int requestCode = this.Intent.GetIntExtra(RequestCodeParamName, 0);
            //StartActivityForResult(intent, requestCode);

            StartActivityForResult(CurrentIntent, CurrentRequestCode);
            CurrentIntent = null;
            CurrentRequestCode = 0;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            Finish();
            var args = new ActivityResultArgs()
            {
                RequestCode = requestCode,
                ResultCode = resultCode,
                Data = data
            };
            ActivityResultReturned?.Invoke(args);
        }

        public static event ActivityResultReturnedDelegate ActivityResultReturned;
    }
}