using System.Net.Mime;
using System.Threading.Tasks;
using IRO.ImprovedWebView.Core;

namespace IRO.Tests.ImprovedWebView.CommonTests
{
    public interface IWebViewTest 
    {
        Task RunTest(IImprovedWebView iwv, ITestingEnvironment env);
    }
}