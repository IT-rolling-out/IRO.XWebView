using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Views;
using Android.Webkit;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Core.Utils;
using IRO.XWebView.Droid.BrowserClients;
using IRO.XWebView.Droid.Containers;
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
        public bool UseDefaultBuilderSettings { get; set; } = true;

        /// <summary>
        /// Can override some builder settings.
        /// </summary>
        public bool UseDefaultWebViewSettings { get; set; } = true;

        public FinestXWebViewProvider(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void ConfigureBuilder(Action<OverriddenFinestWebViewBuilder<TActivityToShow>> action)
        {
            _configureBuilderDelegate = action;
        }

        public virtual async Task<IXWebView> Resolve(XWebViewVisibility preferredVisibility = XWebViewVisibility.Hidden)
        {
            var builder = new OverriddenFinestWebViewBuilder<TActivityToShow>(_context);
            ApplyDefaultBuilderSettings(builder);
            _configureBuilderDelegate?.Invoke(builder);
            builder.Show("about:blank");
            await builder.WaitWebViewShowed();
            var container = new SelfWebViewContainer();
            XWebViewThreadSync.Inst.Invoke(() =>
            {
                var activity = builder.CurrentActivity;
                var wv = activity.PublicWebView;
                var origWebViewClient = wv.WebViewClient;
                var origWebChromeClient = wv.WebChromeClient;
                container.SetWebView(wv, activity);
                var proxyWebViewClient = wv.ProxyWebViewClient();
                var proxyWebChromeClient = wv.ProxyWebChromeClient();
                JoinWebViewClient(proxyWebViewClient.EventsProxy, origWebViewClient);
                JoinWebChromeClient(proxyWebChromeClient.EventsProxy, origWebChromeClient);
                activity.OnClientsEventsProxySetup();
                if (UseDefaultWebViewSettings)
                    wv.ApplyDefaultSettings();
            });
            var xwv = await AndroidXWebView.Create(container);
            return xwv;
        }

        protected virtual void ApplyDefaultBuilderSettings(OverriddenFinestWebViewBuilder<TActivityToShow> builder)
        {

            builder
                .WebViewBuiltInZoomControls(true)
                .WebViewDisplayZoomControls(true)
                .StatusBarColorRes(Resource.Color.bluePrimaryDark)
                .ToolbarColorRes(Resource.Color.bluePrimary)
                .TitleColorRes(Resource.Color.finestWhite)
                .UrlColorRes(Resource.Color.bluePrimaryLight)
                .IconDefaultColorRes(Resource.Color.finestWhite)
                .ProgressBarColorRes(Resource.Color.finestWhite)
                .ProgressBarHeight(4)
                .StringResCopiedToClipboard(Resource.String.copied_to_clipboard)
                .StringResCopiedToClipboard(Resource.String.copied_to_clipboard)
                .StringResCopiedToClipboard(Resource.String.copied_to_clipboard)
                .ShowSwipeRefreshLayout(true)
                .SwipeRefreshColorRes(Resource.Color.bluePrimaryDark)
                .MenuSelector(Resource.Drawable.selector_light_theme)
                .MenuTextPaddingRightRes(Resource.Dimension.defaultMenuTextPaddingLeft)
                .SetCustomAnimations(
                    Resource.Animation.slide_left_in,
                    Resource.Animation.hold,
                    Resource.Animation.hold,
                    Resource.Animation.slide_right_out
                );


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