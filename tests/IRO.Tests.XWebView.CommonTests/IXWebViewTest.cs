using System.Threading.Tasks;
using IRO.XWebView.Core;

namespace IRO.Tests.XWebView.CommonTests
{
    public interface IXWebViewTest
    {
        Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env);
    }
}