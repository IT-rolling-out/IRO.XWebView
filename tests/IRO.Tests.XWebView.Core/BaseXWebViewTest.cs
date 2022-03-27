using System;
using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core
{
    public abstract class BaseXWebViewTest
    {
        protected IXWebViewProvider XWVProvider { get; private set; }
        protected TestAppSetupConfigs AppConfigs { get; private set; }
        ITestingEnvironment _testEnv { get; set; }

        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment testEnv, TestAppSetupConfigs appConfigs)
        {
            XWVProvider = xwvProvider;
            _testEnv = testEnv;
            AppConfigs = appConfigs;
            var countWas = XWVProvider.ProvidedXWebView.Count;
            try
            {
                ShowMessage($"INF: Started test '{this.GetType().Name}'.");
                await RunTest();
                ShowMessage($"INF: Test '{this.GetType().Name}' finished successfully.");
            }
            catch(Exception ex)
            {
                ShowMessage($"ERR: Test '{this.GetType().Name}' finished with error.");
                ShowError(ex.ToString());
                throw;
            }
            finally
            {

                ShowMessage("INF: Waiting all provided XWV disposed.");
                while (XWVProvider.ProvidedXWebView.Count > countWas)
                {
                    await Task.Delay(200);
                }
                ShowMessage("INF: All provided XWV successfully disposed.");
            }

        }

        protected abstract Task RunTest();

        protected void ShowMessage(string str)
        {
            _testEnv.Message(str);
        }

        protected void ShowError(string str)
        {
            _testEnv.Error(str);
        }

        protected void Assert_Equal(object obj1, object obj2)
        {
            if (!obj1.Equals(obj2))
            {
                throw new Exception($"{nameof(Assert_Equal)} error. Value '{obj1}' must be EQUAL to '{obj2}'.");
            }
            ShowMessage($"EQUAL CORRECT {obj1}=={obj2}");
        }
        protected void Assert_NotEqual(object obj1, object obj2)
        {
            if (obj1.Equals(obj2))
            {
                throw new Exception($"{nameof(Assert_NotEqual)} error. Value '{obj1}' must be NOT EQUAL to '{obj2}'.");
            }
            ShowMessage($"NOT EQUAL CORRECT {obj1}!={obj2}");
        }
    }
}