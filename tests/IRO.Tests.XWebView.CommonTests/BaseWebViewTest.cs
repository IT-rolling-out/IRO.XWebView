using System.Net.Mime;
using System.Threading.Tasks;
using IRO.XWebView.Core;

namespace IRO.Tests.XWebView.CommonTests
{
    public interface IWebViewTest 
    {
        Task RunTest(IXWebView xwv, ITestingEnvironment env);
    }
}