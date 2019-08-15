using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;

namespace IRO.Tests.XWebView.Core.JsInterfaces
{
    public class TestsMainMenuJsInterface
    {
        readonly CommonTestXWebViewProvider _provider;

        readonly ITestingEnvironment _testingEnvironment;

        public TestsMainMenuJsInterface(CommonTestXWebViewProvider provider, ITestingEnvironment testingEnvironment)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _testingEnvironment = testingEnvironment;
        }

        async Task RunXWebViewTest<TWebViewTest>()
            where TWebViewTest : IXWebViewTest
        {
            //In test we use overrided NewActivityXWebViewProvider to get link to last XWebView.
            //In your projects you can use NewActivityXWebViewProvider.
            //Another use case is to use XWebViewProvider by object, when you created XWebView on current activity.
            var test = Activator.CreateInstance<TWebViewTest>();
            try
            {
                await test.RunTest(_provider, _testingEnvironment);
                if (_provider.LastVisibility == XWebViewVisibility.Hidden)
                {
                    //Crunch to dispose transparent XWebView.
                    //You can do it saving link from IXWebViewProvider.
                    _testingEnvironment.Message("Hidden XWebView disposed automatically.");
                    _provider.LastResolved.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR \n" + ex.ToString());
                _testingEnvironment.Error(ex.ToString());
            }
        }
    }
}
