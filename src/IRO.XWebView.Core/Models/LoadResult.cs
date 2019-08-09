namespace IRO.XWebView.Core.Models
{
    public class LoadResult
    {
        public LoadResult()
        {
        }

        public LoadResult(string url)
        {
            Url = url;
        }

        public string Url { get; set; }
    }
}