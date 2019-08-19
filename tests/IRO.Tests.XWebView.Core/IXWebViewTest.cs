using System.Threading.Tasks;
using IRO.XWebView.Core.Providers;

namespace IRO.Tests.XWebView.Core
{
    public interface IXWebViewTest
    {
        Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env, TestAppSetupConfigs appConfigs);
    }
}