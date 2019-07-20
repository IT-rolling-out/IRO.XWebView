using System;
using Android.Graphics;
using Android.Runtime;
using Android.Webkit;
using IRO.ImprovedWebView.Core.EventsAndDelegates;

namespace IRO.ImprovedWebView.Droid.EventsProxy
{
    /// <summary>
    /// Настройка браузера.
    /// </summary>
    class ProxyWebViewClient : WebViewClient
    {
        readonly WebViewEventsProxy _proxy;

        LoadStartedEventArgs _lastLoadStartedArgs;

        LoadFinishedEventArgs _errorLoadArgs;

        bool _oldShouldOverrideUrlLoadingWorks = false;

        bool _wasHandled;

        public ProxyWebViewClient(WebViewEventsProxy proxy)
        {
            _proxy = proxy;
            _lastLoadStartedArgs = new LoadStartedEventArgs()
            {
                Url = "about:blank"
            };
        }

        public override void OnPageCommitVisible(WebView view, string url)
        {
            _proxy.OnPageCommitVisible(view, url);
            var args = _errorLoadArgs
                ?? new LoadFinishedEventArgs()
                {
                    Url = url,
                    IsError = false,
                    WasHandled=_wasHandled
                };
            _errorLoadArgs = null;
            OnLoadFinished(args);
            base.OnPageCommitVisible(view, url);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            _proxy.OnPageFinished(view, url);
            base.OnPageFinished(view, url);
        }

        public override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            _proxy.OnPageStarted(view, url,favicon);
            _lastLoadStartedArgs = new LoadStartedEventArgs()
            {
                Url = url
            };
            OnLoadStarted(_lastLoadStartedArgs);
            base.OnPageStarted(view, url, favicon);
        }

        [Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            _proxy.ShouldOverrideUrlLoading(view, url);
            _oldShouldOverrideUrlLoadingWorks = true;
            if (_lastLoadStartedArgs.Handled)
            {
                _wasHandled = true;
                return true;
            }
            else
            {
                _wasHandled = false;
            }
            return base.ShouldOverrideUrlLoading(view, url);
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            _proxy.ShouldOverrideUrlLoading2(view, request);
            if (!_oldShouldOverrideUrlLoadingWorks && _lastLoadStartedArgs.Handled)
            {
                _wasHandled = true;
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
            if (errorCode == ClientError.Connect && !failingUrl.Contains("favicon"))
            {
                //If real error.
                _errorLoadArgs = new LoadFinishedEventArgs()
                {
                    Url = failingUrl,
                    IsError = true,
                    ErrorDescription = description
                };
            }
            else
                base.OnReceivedError(view, errorCode, description, failingUrl);
        }

        public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error)
        {
            _proxy.OnReceivedError2(view, request, error);
            if (error.ErrorCode == ClientError.Connect && request.IsForMainFrame && !request.Url.ToString().Contains("favicon"))
            {
                //If real error.
                _errorLoadArgs = new LoadFinishedEventArgs()
                {
                    Url = request.Url.ToString(),
                    IsError = true,
                    ErrorDescription = error.Description
                };
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

        public event LoadFinishedDelegate LoadFinished;

        void OnLoadStarted(LoadStartedEventArgs args)
        {
            LoadStarted?.Invoke(this, args);
        }

        void OnLoadFinished(LoadFinishedEventArgs args)
        {
            LoadFinished?.Invoke(this, args);
        }
        #endregion
    }
}