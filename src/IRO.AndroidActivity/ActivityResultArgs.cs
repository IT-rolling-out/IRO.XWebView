using Android.App;
using Android.Content;

namespace IRO.AndroidActivity
{
    public class ActivityResultArgs
    {
        public int RequestCode { get; set; }

        public Result ResultCode { get; set; }

        public Intent Data { get; set; }
    }
}