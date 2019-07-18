namespace IRO.ImprovedWebView.Droid
{
    public struct WebViewRendererSettings
    {
        public PermissionsMode PermissionsMode { get; set; }   

        public bool DownloadsEnabled { get; set; }

        public bool UploadsEnabled { get; set; }

        public bool ZoomEnabled { get; set; } 

        public string CacheFolder { get; set; }

        public ProgressBarStyle ProgressBarStyle { get; set; } 
    }
}
