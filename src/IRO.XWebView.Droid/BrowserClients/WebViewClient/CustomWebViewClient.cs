using System;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Net.Http;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Events;

namespace IRO.XWebView.Droid.BrowserClients
{
    public class CustomWebViewClient : WebViewClient
    {
        #region Main.
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
            EventsProxy.RiseOnPageCommitVisible(view, url);
            if (_lastLoadWasOk)
            {
                OnLoadFinished_Ok(url);
            }

            base.OnPageCommitVisible(view, url);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            EventsProxy.RiseOnPageFinished(view, url);
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
            EventsProxy.RiseOnPageStarted(view, url, favicon);
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
            EventsProxy.RiseShouldOverrideUrlLoading(view, url);
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
            EventsProxy.RiseShouldOverrideUrlLoading2(view, request);
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
            EventsProxy.RiseOnReceivedError(view, errorCode, description, failingUrl);
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
            EventsProxy.RiseOnReceivedError2(view, request, error);
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
            EventsProxy.RiseOnLoadResource(view, url);
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

        #endregion


        public override void DoUpdateVisitedHistory(WebView view, string url, bool isReload)
        {
            base.DoUpdateVisitedHistory(view, url, isReload);
            EventsProxy.RiseDoUpdateVisitedHistory(view, url, isReload);
        }

        public override void OnFormResubmission(WebView view, Message dontResend, Message resend)
        {
            base.OnFormResubmission(view, dontResend, resend);
            EventsProxy.RiseOnFormResubmission(view, dontResend, resend);
        }

        public override void OnReceivedClientCertRequest(WebView view, ClientCertRequest request)
        {
            base.OnReceivedClientCertRequest(view, request);
            EventsProxy.RiseOnReceivedClientCertRequest(view, request);
        }

        public override void OnReceivedHttpAuthRequest(WebView view, HttpAuthHandler handler, string host, string realm)
        {
            base.OnReceivedHttpAuthRequest(view, handler, host, realm);
            EventsProxy.RiseOnReceivedHttpAuthRequest(view, handler, host, realm);
        }

        public override void OnReceivedHttpError(WebView view, IWebResourceRequest request, WebResourceResponse errorResponse)
        {
            base.OnReceivedHttpError(view, request, errorResponse);
            EventsProxy.RiseOnReceivedHttpError(view, request, errorResponse);
        }

        public override void OnReceivedLoginRequest(WebView view, string realm, string account, string args)
        {
            base.OnReceivedLoginRequest(view, realm, account, args);
            EventsProxy.RiseOnReceivedLoginRequest(view, realm, account, args);
        }

        public override void OnReceivedSslError(WebView view, SslErrorHandler handler, SslError error)
        {
            base.OnReceivedSslError(view, handler, error);
            EventsProxy.RiseOnReceivedSslError(view, handler, error);
        }

        public override bool OnRenderProcessGone(WebView view, RenderProcessGoneDetail detail)
        {
            var resBase = base.OnRenderProcessGone(view, detail);
            var res = EventsProxy.RiseOnRenderProcessGone(view, detail);
            return res ?? resBase;
        }

        public override void OnSafeBrowsingHit(WebView view, IWebResourceRequest request,
            [GeneratedEnum] SafeBrowsingThreat threatType, SafeBrowsingResponse callback)
        {
            base.OnSafeBrowsingHit(view, request, threatType, callback);
            EventsProxy.RiseOnSafeBrowsingHit(view, request, threatType, callback);
        }

        public override void OnScaleChanged(WebView view, float oldScale, float newScale)
        {
            base.OnScaleChanged(view, oldScale, newScale);
            EventsProxy.RiseOnScaleChanged(view, oldScale, newScale);
        }

        [Obsolete("deprecated")]
        public override void OnTooManyRedirects(WebView view, Message cancelMsg, Message continueMsg)
        {
            base.OnTooManyRedirects(view, cancelMsg, continueMsg);
            EventsProxy.RiseOnTooManyRedirects(view, cancelMsg, continueMsg);
        }

        public override void OnUnhandledInputEvent(WebView view, InputEvent e)
        {
            base.OnUnhandledInputEvent(view, e);
            EventsProxy.RiseOnUnhandledInputEvent(view, e);
        }

        public override void OnUnhandledKeyEvent(WebView view, KeyEvent e)
        {
            base.OnUnhandledInputEvent(view, e);
            EventsProxy.RiseOnUnhandledInputEvent(view, e);
        }

        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            var resBase = base.ShouldInterceptRequest(view, request);
            var res = EventsProxy.RiseShouldInterceptRequest(view, request);
            return res ?? resBase;
        }

        [Obsolete("deprecated")]
        public override WebResourceResponse ShouldInterceptRequest(WebView view, string url)
        {
            var resBase = base.ShouldInterceptRequest(view, url);
            var res = EventsProxy.RiseShouldInterceptRequest2(view, url);
            return res ?? resBase;
        }

        public override bool ShouldOverrideKeyEvent(WebView view, KeyEvent e)
        {
            var resBase = base.ShouldOverrideKeyEvent(view, e);
            var res = EventsProxy.RiseShouldOverrideKeyEvent(view, e);
            return res ?? resBase;
        }
    }
}
