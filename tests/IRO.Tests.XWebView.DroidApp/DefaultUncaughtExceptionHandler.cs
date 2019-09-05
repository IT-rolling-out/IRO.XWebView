using Java.Lang;

namespace IRO.Tests.XWebView.DroidApp
{
    public class DefaultUncaughtExceptionHandler :  Java.Lang.Object, Thread.IUncaughtExceptionHandler
    {
        public void UncaughtException(Thread t, Throwable e)
        {
            var msg = "---------JAVA ERROR---------\n" + e.ToString();
            System.Diagnostics.Debug.WriteLine(msg);
        }
    }
}