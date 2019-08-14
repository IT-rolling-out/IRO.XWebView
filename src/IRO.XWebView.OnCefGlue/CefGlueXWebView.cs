using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Chromely.CefGlue.Browser;
using Chromely.CefGlue.Browser.EventParams;
using IRO.XWebView.Core;
using IRO.XWebView.Core.BindingJs;
using IRO.XWebView.Core.BindingJs.LowLevelBridge;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Events;
using IRO.XWebView.Core.Models;
using Xilium.CefGlue;

namespace IRO.XWebView.OnCefGlue
{
    public class CefGlueXWebView : BaseXWebView
    {
        public const string CallbacksStartsWith = "CALLBACK_MESSAGE_";

        readonly UnifiedLowLevelBridge _bridge;

        public CefGlueBrowser CefGlueBrowser { get; }

        public CefGlueXWebView(CefGlueBrowser cefGlueBrowser)
        {
            CefGlueBrowser = cefGlueBrowser;
            _bridge = new UnifiedLowLevelBridge(this.BindingJsSystem, this);
            CefGlueBrowser.ConsoleMessage += ConsoleMessageHandler;
            // ReSharper disable once VirtualMemberCallInConstructor
            RegisterEvents();
        }

        /// <summary>
        /// If true - you can change visibility after creation.
        /// If false - <see cref="P:IRO.XWebView.Core.IXWebView.Visibility" /> assignment will throw exception.
        /// </summary>
        public override bool CanSetVisibility { get; } = true;

        /// <summary>
        /// Execute your script in browser without any manipulations.
        /// Doesn't support promises.
        /// </summary>
        public override async Task<string> ExJsDirect(string script, int? timeoutMS = null)
        {
            return await CefGlueBrowser.CefBrowser.ExecuteJavascript(script, timeoutMS);
        }

        public override void Stop()
        {
            CefGlueBrowser.CefBrowser.StopLoad();
        }

        public override void ClearCookies()
        {
            throw new NotImplementedException();
        }

        protected override void StartLoading(string url)
        {
            CefGlueBrowser.CefBrowser.GetMainFrame().LoadUrl(url);

        }

        protected override void StartLoadingHtml(string data, string baseUrl)
        {
            base.StartLoadingHtml(data, baseUrl);
            CefGlueBrowser.CefBrowser.GetMainFrame().LoadString(data, baseUrl);
        }

        protected override void ToggleVisibilityState(XWebViewVisibility visibility)
        {
            throw new NotImplementedException();
        }

        public override bool CanGoForward()
        {
            return CefGlueBrowser.CefBrowser.CanGoForward;
        }

        public override bool CanGoBack()
        {
            return CefGlueBrowser.CefBrowser.CanGoBack;
        }

        /// <summary>
        /// Return <see cref="CefGlueBrowser"/>.
        /// </summary>
        /// <returns></returns>
        public override object Native()
        {
            return CefGlueBrowser;
        }

        #region Js bridge.
        public override async Task AttachBridge()
        {
            //Registering unified OnJsCall.
            var jsObjName = Core.BindingJs.BindingJsSystem.JsBridgeObjectName;
            var script = $@"
window.{jsObjName} = window.{jsObjName} || {{}};
window.{jsObjName}.{nameof(UnifiedLowLevelBridge.OnJsCall)} = function(str){{
  console.log({CallbacksStartsWith} + str);
}}
";
            await ExJsDirect(script);
            await base.AttachBridge();
        }

        void ConsoleMessageHandler(object sender, ConsoleMessageEventArgs eventArgs)
        {
            if (eventArgs.Message.StartsWith(CallbacksStartsWith))
            {
                eventArgs.Handled = true;
                var passedStr = eventArgs.Message.Substring(CallbacksStartsWith.Length);
                OnJsCall(passedStr);
            }
        }

        void OnJsCall(string passedStr)
        {
            _bridge.OnJsCall(passedStr);
        }
        #endregion

        protected virtual void RegisterEvents()
        {
            string newAddress = "about:blank";
            CefGlueBrowser.AddressChanged += (s, a) =>
            {
                if (!a.Frame.IsMain)
                    return;
                newAddress = a.Address;
            };
            CefGlueBrowser.FrameLoadStart += (s, a) =>
            {
                if (!a.Frame.IsMain)
                    return;
                var loadStartArgs = new LoadStartedEventArgs()
                {
                    Url = newAddress
                };
                OnLoadStarted(loadStartArgs);
                //!Load cancel not implemented.
            };
            CefGlueBrowser.FrameLoadEnd += (s, a) =>
            {
                if (!a.Frame.IsMain)
                    return;
                var loadStartArgs = new LoadFinishedEventArgs()
                {
                    Url = a.Frame.Url
                };
                OnLoadFinished(loadStartArgs);
            };
            CefGlueBrowser.LoadError += (s, a) =>
            {
                if (!a.Frame.IsMain)
                    return;
                var loadStartArgs = new LoadFinishedEventArgs()
                {
                    Url = a.FailedUrl,
                    ErrorType = a.ErrorCode.ToString(),
                    ErrorDescription = a.ErrorText,
                    IsError = true
                };
                OnLoadFinished(loadStartArgs);
            };
            CefGlueBrowser.RenderProcessTerminated += delegate
            {
                Dispose();
            };
        }
    }
}
