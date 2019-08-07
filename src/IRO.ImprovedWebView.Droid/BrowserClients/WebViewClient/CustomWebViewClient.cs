using System;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Runtime;
using Android.Webkit;
using IRO.ImprovedWebView.Core;
using IRO.ImprovedWebView.Core.EventsAndDelegates;

namespace IRO.ImprovedWebView.Droid
{
    public class CustomWebViewClient : Android.Webkit.WebViewClient
    {
        readonly WebViewEventsProxy _proxy;

        LoadStartedEventArgs _lastLoadStartedArgs;

        bool _lastLoadWasOk = true;

        bool _oldShouldOverrideUrlLoadingWorks;

        bool _pageCommitVisibleNotSupported=true;

        public CustomWebViewClient(WebViewEventsProxy proxy)
        {
            _proxy = proxy;
            _lastLoadStartedArgs = new LoadStartedEventArgs()
            {
                Url = "about:blank"
            };
        }

        public override void OnPageCommitVisible(WebView view, string url)
        {
            _pageCommitVisibleNotSupported = false;
            _proxy.OnPageCommitVisible(view, url);
            if (_lastLoadWasOk)
            {
                OnLoadFinished_Ok(url);
            }
            base.OnPageCommitVisible(view, url);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            _proxy.OnPageFinished(view, url);
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
            _proxy.OnPageStarted(view, url, favicon);
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
            _proxy.ShouldOverrideUrlLoading(view, url);
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
            _proxy.ShouldOverrideUrlLoading2(view, request);
            if (!_oldShouldOverrideUrlLoadingWorks && _lastLoadStartedArgs.Cancel)
            {
                 //Cancel load.
                return true;
            }
            return base.ShouldOverrideUrlLoading(view, request);
        }

        [Obsolete]
        public override void OnReceivedError(WebView view, [GeneratedEnum] ClientError errorCode, string description, string failingUrl)
        {
            _proxy.OnReceivedError(view, errorCode, description, failingUrl);
            //Вроде как этот метод работает до апи 23, а в последующих работает второй OnReceivedError.
            //Не уверен что он срабатывает только для страниц, нужно тестирование.
            if (errorCode == ClientError.Connect && !failingUrl.Contains("favicon.ico"))
            {
                //If real error.
                OnLoadFinished_Error(
                    failingUrl,
                    description
                    );
            }
            else
                base.OnReceivedError(view, errorCode, description, failingUrl);
        }

        public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error)
        {
            _proxy.OnReceivedError2(view, request, error);
            if (error.ErrorCode == ClientError.Connect && request.IsForMainFrame && !request.Url.ToString().Contains("favicon.ico"))
            {
                //If real error.
                OnLoadFinished_Error(
                    request.Url.ToString(),
                    error.Description
                    );
            }
            else
                base.OnReceivedError(view, request, error);
        }

        public override void OnLoadResource(WebView view, string url)
        {
            _proxy.OnLoadResource(view, url);
            base.OnLoadResource(view, url);
        }

        #region Events.
        public event LoadStartedDelegate LoadStarted;

        /// <summary>
        /// Represend <see cref="IImprovedWebView.LoadFinished"/> event, but 
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

        void OnLoadFinished_Error(string url, string errorDescription)
        {
            var args = new LoadFinishedEventArgs()
            {
                Url = url,
                IsError = true,
                ErrorDescription = errorDescription
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