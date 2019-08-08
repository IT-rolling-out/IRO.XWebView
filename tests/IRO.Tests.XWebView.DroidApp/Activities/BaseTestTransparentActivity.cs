using System.Threading.Tasks;
using IRO.XWebView.Droid;
using IRO.XWebView.Droid.Renderer;

namespace IRO.Tests.XWebView.DroidApp.Activities
{
    public abstract class BaseTestTransparentActivity : XWebViewTransparentActivity
    {
        protected readonly AndroidTestingEnvironment TestingEnvironment;

        protected BaseTestTransparentActivity()
        {
            TestingEnvironment = new AndroidTestingEnvironment(this);
        }

        public override async Task WebViewWrapped(AndroidXWebView XWebView)
        {
            await base.WebViewWrapped(XWebView);
            try
            {
                await RunTest(XWebView);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR \n" + ex.ToString());
                TestingEnvironment.Error(ex.ToString());
            }


        }

        protected abstract Task RunTest(AndroidXWebView iwv);
    }
}