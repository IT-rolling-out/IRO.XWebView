namespace IRO.Tests.XWebView.Core
{
    /// <summary>
    /// Simple custom testing environment to execute integration tests on different platforms.
    /// </summary>
    public interface ITestingEnvironment
    {
        void Message(string str);

        void Error(string str);
    }
}