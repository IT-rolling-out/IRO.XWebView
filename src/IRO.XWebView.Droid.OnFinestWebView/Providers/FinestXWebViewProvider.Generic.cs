using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Webkit;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Droid.BrowserClients;
using IRO.XWebView.Droid.Utils;

namespace IRO.XWebView.Droid.OnFinestWebView.Providers
{
    public class FinestXWebViewProvider<TActivityToShow> : IXWebViewProvider
        where TActivityToShow : OverriddenFinestWebViewActivity
    {
        readonly Context _context;

        Action<OverriddenFinestWebViewBuilder<TActivityToShow>> _configureBuilderDelegate;

        /// <summary>
        /// Default is true.
        /// </summary>
        public bool UseBackButtonCrunch { get; set; } = true;

        public FinestXWebViewProvider(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void ConfigureBuilder(Action<OverriddenFinestWebViewBuilder<TActivityToShow>> action)
        {
            _configureBuilderDelegate = action;
        }

        public virtual async Task<IXWebView> Resolve(XWebViewVisibility prefferedVisibility = XWebViewVisibility.Hidden)
        {
            var builder = new OverriddenFinestWebViewBuilder<TActivityToShow>(_context);
            ApplyDefaultBuilderSettings(builder);
            _configureBuilderDelegate?.Invoke(builder);
            builder.Show("about:blank");
            await builder.WaitWebViewShowed();
            var activity = builder.CurrentActivity;
            var wv = activity.PublicWebView;
            var origWebViewClient = wv.WebViewClient;
            var origWebChromeClient = wv.WebChromeClient;
            var container = new SelfWebViewContainer();
            container.SetWebView(wv, activity);
            var proxyWebViewClient = wv.ProxyWebViewClient();
            var proxyWebChromeClient = wv.ProxyWebChromeClient();
            JoinWebViewClient(proxyWebViewClient.EventsProxy, origWebViewClient);
            JoinWebChromeClient(proxyWebChromeClient.EventsProxy, origWebChromeClient);
            activity.OnClientsEventsProxySetup();
            var xwv = await AndroidXWebView.Create(container);
            ApplyViewSettings(activity, wv, xwv);
            return xwv;
        }

        protected virtual void ApplyDefaultBuilderSettings(OverriddenFinestWebViewBuilder<TActivityToShow> builder)
        {

        }

        protected virtual void ApplyViewSettings(TActivityToShow activity, WebView wv, AndroidXWebView xwv)
        {

        }

        void JoinWebViewClient(WebViewClientEventsProxy ep, WebViewClient client)
        {
            ep.OnPageStarted += client.OnPageStarted;
            ep.OnPageFinished += client.OnPageFinished;
            ep.ShouldOverrideUrlLoading += (v, req) =>
            {
                client.ShouldOverrideUrlLoading(v, req);
            };
            ep.ShouldOverrideUrlLoading2 += (v, url) =>
            {
                client.ShouldOverrideUrlLoading(v, url);
            };
            ep.OnLoadResource += client.OnLoadResource;
            ep.OnPageCommitVisible += client.OnPageCommitVisible;
        }

        void JoinWebChromeClient(WebChromeClientEventsProxy ep, WebChromeClient client)
        {
            ep.OnProgressChanged += client.OnProgressChanged;
            ep.OnReceivedTitle += client.OnReceivedTitle;
            ep.OnReceivedTouchIconUrl += client.OnReceivedTouchIconUrl;
        }
    }
}