using System;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Runtime;
using Android.Webkit;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Events;

namespace IRO.XWebView.Droid.BrowserClients
{
    public class CustomWebViewClient : WebViewClient
    {
        LoadStartedEventArgs _lastLoadStartedArgs;

        bool _lastLoadWasOk = true;

        bool _oldShouldOverrideUrlLoadingWorks;

        bool _pageCommitVisibleNotSupported = true;

        public CustomWebViewClient()
        {
            _lastLoadStartedArgs = new LoadStartedEventArgs()
            {
                Url = "about:blank"
            };
        }

        public WebViewClientEventsProxy EventsProxy { get; } = new WebViewClientEventsProxy();

        public override void OnPageCommitVisible(WebView view, string url)
        {
            _pageCommitVisibleNotSupported = false;
            EventsProxy.OnPageCommitVisible(view, url);
            if (_lastLoadWasOk)
            {
                OnLoadFinished_Ok(url);
            }

            base.OnPageCommitVisible(view, url);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            EventsProxy.OnPageFinished(view, url);
            //PageCommitVisible event choosed as LoadFinished trigger, because it looks more
            //more predictable when we load pages.
            //Unfortunately it doesn't work on old OS vesions (for example, my android 6.0).
            //In this case we use OnPageFinished.
            if (_pageCommitVisibleNotSupported && _lastLoadWasOk)
            {
                //And we wait 200ms before fairing event and hoping that it will really render.
                Task.Run(async () =>
                {
                    await Task.Delay(200);
                    OnLoadFinished_Ok(url);
                });
            }

            base.OnPageFinished(view, url);
        }

        public override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            EventsProxy.OnPageStarted(view, url, favicon);
            _lastLoadStartedArgs = new LoadStartedEventArgs()
            {
                Url = url
            };
            OnLoadStarted(_lastLoadStartedArgs);
            if (_lastLoadStartedArgs.Cancel)
            {
                //Emulate load finishing.
                OnLoadFinished_Cancel(url);
            }

            base.OnPageStarted(view, url, favicon);
        }

        [Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            EventsProxy.ShouldOverrideUrlLoading(view, url);
            _oldShouldOverrideUrlLoadingWorks = true;
            if (_lastLoadStartedArgs.Cancel)
            {
                //Cancel load.
                return true;
            }

            return base.ShouldOverrideUrlLoading(view, url);
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            EventsProxy.ShouldOverrideUrlLoading2(view, request);
            if (!_oldShouldOverrideUrlLoadingWorks && _lastLoadStartedArgs.Cancel)
            {
                //Cancel load.
                return true;
            }

            return base.ShouldOverrideUrlLoading(view, request);
        }

        [Obsolete]
        public override void OnReceivedError(WebView view, [GeneratedEnum] ClientError errorCode, string description,
            string failingUrl)
        {
            EventsProxy.OnReceivedError(view, errorCode, description, failingUrl);
            //Вроде как этот метод работает до апи 23, а в последующих работает второй OnReceivedError.
            //Не уверен что он срабатывает только для страниц, нужно тестирование.
            if (!failingUrl.Contains("favicon.ico"))
            {
                //If real error.
                OnLoadFinished_Error(
                    failingUrl,
                    description,
                    errorCode.ToString()
                );
            }
            else
                base.OnReceivedError(view, errorCode, description, failingUrl);
        }

        public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error)
        {
            EventsProxy.OnReceivedError2(view, request, error);
            if (request.IsForMainFrame && !request.Url.ToString().Contains("favicon.ico"))
            {
                //If real error.
                OnLoadFinished_Error(
                    request.Url.ToString(),
                    error.Description,
                    error.ErrorCode.ToString()
                );
            }
            else
                base.OnReceivedError(view, request, error);
        }

        public override void OnLoadResource(WebView view, string url)
        {
            EventsProxy.OnLoadResource(view, url);
            base.OnLoadResource(view, url);
        }

        #region Events.

        public event LoadStartedDelegate LoadStarted;

        /// <summary>
        /// Represend <see cref="IXWebView.LoadFinished"/> event, but 
        /// really it invoked in OnPageCommitVisible, when page rendered.
        /// </summary>
        public event LoadFinishedDelegate LoadFinished;

        void OnLoadStarted(LoadStartedEventArgs args)
        {
            LoadStarted?.Invoke(this, args);
        }

        void OnLoadFinished_Cancel(string url)
        {
            var args = new LoadFinishedEventArgs()
            {
                Url = url,
                WasCancelled = true
            };
            OnLoadFinished(args);
            _lastLoadWasOk = false;
        }

        void OnLoadFinished_Error(string url, string errorDescription, string errorType)
        {
            var args = new LoadFinishedEventArgs()
            {
                Url = url,
                IsError = true,
                ErrorDescription = errorDescription,
                ErrorType = errorType
            };
            OnLoadFinished(args);
            _lastLoadWasOk = false;
        }

        void OnLoadFinished_Ok(string url)
        {
            var args = new LoadFinishedEventArgs()
            {
                Url = url
            };
            OnLoadFinished(args);
            _lastLoadWasOk = true;
        }

        void OnLoadFinished(LoadFinishedEventArgs args)
        {
            LoadFinished?.Invoke(this, args);
        }

        #endregion
    }
}