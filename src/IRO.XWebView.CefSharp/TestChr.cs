namespace IRO.XWebView.CefSharp
{
    internal class TestChr
    {
        public TestChr()
        {
        }

        public string Test1()
        {
            return "Test1";
        }

        public async Task<string> Test2()
        {
            return "Test2";
        }

        public async Task<string> Test3()
        {
            return await Task.Run(() =>
            {
                return "Test3";
            });

        }

        public Task<string> Test4()
        {
            return Task.Run(() =>
            {
                return "Test4";
            });

        }
    }
}