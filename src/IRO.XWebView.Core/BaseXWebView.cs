using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using IRO.XWebView.Core.BindingJs;
using IRO.XWebView.Core.EventsAndDelegates;
using IRO.XWebView.Core.Models;

namespace IRO.XWebView.Core
{
    public abstract class BaseXWebView : IXWebView
    {
        bool _isBusy;
        string _url;

        protected BaseXWebView()
        {
            BindingJsSystem = new BindingJsSystem();
            LoadStarted += (s, a) => { _isBusy = true; };
            LoadFinished += (s, a) =>
            {
                _url = a.Url;
                _isBusy = false;
            };
        }

        protected BindingJsSystem BindingJsSystem { get; }

        /// <summary>
        /// Base version use backing field to set, so you can override it.
        /// </summary>
        public virtual string Url => _url;

        /// <summary>
        /// Base version use backing field to set, so you can override it.
        /// </summary>
        public virtual bool IsBusy => _isBusy;

        public abstract string BrowserType { get; }

        public abstract XWebViewVisibility Visibility { get; set; }

        public IDictionary<string, object> Data { get; } = new Dictionary<string, object>();

        public virtual async Task<LoadResult> LoadUrl(string url)
        {
            var finishedEventArgs = await CreateLoadFinishedTask(() => { StartLoading(url); });
            return new LoadResult(finishedEventArgs.Url);
        }

        public virtual async Task<LoadResult> LoadHtml(string html, string baseUrl = "about:blank")
        {
            var finishedEventArgs = await CreateLoadFinishedTask(() => { StartLoadingHtml(html, baseUrl); });
            return new LoadResult(finishedEventArgs.Url);
        }

        public virtual async Task<LoadResult> Reload()
        {
            var finishedEventArgs = await CreateLoadFinishedTask(
                StartReloading
                );
            return new LoadResult(finishedEventArgs.Url);
        }

        public virtual async Task<LoadResult> GoForward()
        {
            var canGoForward = CanGoForward();
            var args = new GoForwardEventArgs();
            args.CanGoForward = canGoForward;
            OnGoForwardRequested(args);
            if (args.Cancel)
                throw new TaskCanceledException("Go forward cancelled.");
            var finishedEventArgs = await CreateLoadFinishedTask(StartGoForward);
            return new LoadResult(finishedEventArgs.Url);
        }

        public virtual async Task<LoadResult> GoBack()
        {
            var canGoBack = CanGoBack();
            var args = new GoBackEventArgs();
            args.CanGoBack = canGoBack;
            OnGoBackRequested(args);
            if (args.Cancel)
                throw new TaskCanceledException("Go back cancelled.");
            var finishedEventArgs = await CreateLoadFinishedTask(StartGoBack);
            return new LoadResult(finishedEventArgs.Url);
        }

        public virtual async Task AttachBridge()
        {
            var script = BindingJsSystem.GetAttachBridgeScript();
            await ExJsDirect(script);
        }

        public async Task WaitWhileBusy()
        {
            if (!IsBusy)
                return;
            if (_pageFinishedSync_TaskCompletionSource == null)
                //Create new.
                await CreateLoadFinishedTask(() => { });
            else
                //Await previous.
                await _pageFinishedSync_TaskCompletionSource.Task;
        }

        public void BindToJs(MethodInfo methodInfo, object invokeOn, string functionName, string jsObjectName)
        {
            BindingJsSystem.BindToJs(methodInfo, invokeOn, functionName, jsObjectName);
        }

        public void UnbindFromJs(string functionName, string jsObjectName)
        {
            BindingJsSystem.UnbindFromJs(functionName, jsObjectName);
        }

        public virtual async Task<TResult> ExJs<TResult>(string script, bool promiseResultSupport = false,
            int? timeoutMS = null)
        {
            return await BindingJsSystem.ExJs<TResult>(this, script, promiseResultSupport, timeoutMS);
        }

        protected virtual void StartGoForward()
        {
            var script = "window.history.forward();";
            ExJsDirect(script);
        }

        protected virtual void StartGoBack()
        {
            var script = "window.history.back();";
            ExJsDirect(script);
        }

        protected virtual void StartReloading()
        {
            var script = "document.location.reload(true);";
            ExJsDirect(script);
        }

        public abstract Task<string> ExJsDirect(string script, int? timeoutMS = null);

        public abstract void Stop();

        public abstract bool CanGoForward();

        public abstract bool CanGoBack();

        public abstract object Native();

        public abstract void Dispose();

        protected abstract void StartLoading(string url);

        protected abstract void StartLoadingHtml(string data, string baseUrl);

        #region Events.
        public event GoBackDelegate GoBackRequested;

        public event GoForwardDelegate GoForwardRequested;

        public event LoadStartedDelegate LoadStarted;

        public event LoadFinishedDelegate LoadFinished;

        public event Action<object, EventArgs> Disposing;

        public event Action<object, EventArgs> Disposed;

        protected void OnGoBackRequested(GoBackEventArgs args)
        {
            GoBackRequested?.Invoke(this, args);
        }

        protected void OnGoForwardRequested(GoForwardEventArgs args)
        {
            GoForwardRequested?.Invoke(this, args);
        }

        protected internal void OnLoadStarted(LoadStartedEventArgs args)
        {
            LoadStarted?.Invoke(this, args);
        }

        protected internal void OnLoadFinished(LoadFinishedEventArgs args)
        {
            LoadFinished?.Invoke(this, args);
        }

        protected void OnDisposing()
        {
            Disposing?.Invoke(this, EventArgs.Empty);
        }

        protected void OnDisposed()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Load finished sync part.
        readonly object _pageFinishedSync_Locker = new object();

        TaskCompletionSource<LoadFinishedEventArgs> _pageFinishedSync_TaskCompletionSource;

        /// <summary>
        ///  Execute pased action and return first page load result.
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        async Task<LoadFinishedEventArgs> CreateLoadFinishedTask(Action act)
        {
            //Локкер нужен для того, чтоб обязательно вернуть нужный таск из метода, даже если он сразу будет отменен.
            lock (_pageFinishedSync_Locker)
            {
                Stop();
                TryCancelPageFinishedTask();
                var tcs = new TaskCompletionSource<LoadFinishedEventArgs>(
                    TaskContinuationOptions.RunContinuationsAsynchronously
                );
                _pageFinishedSync_TaskCompletionSource = tcs;
                LoadFinishedDelegate loadFinishedHandler = null;
                loadFinishedHandler = (s, a) =>
                {
                    LoadFinished -= loadFinishedHandler;
                    if (a.IsError)
                    {
                        Debug.WriteLine("XWebView error: 'load exception'");
                        tcs?.TrySetException(
                            new NotImplementedException($"Load exception: {a.ErrorDescription} .")
                        );
                    }
                    else if (a.WasCancelled)
                    {
                        tcs?.TrySetException(new TaskCanceledException("Load was cancelled"));
                    }
                    else
                    {
                        tcs?.TrySetResult(a);
                    }
                };
                LoadFinished += loadFinishedHandler;
                act();
            }

            await _pageFinishedSync_TaskCompletionSource.Task.ConfigureAwait(false);
            return await _pageFinishedSync_TaskCompletionSource.Task;
        }

        /// <summary>
        ///  Вызывается для удаления ссылок на обработчик события и промиса.
        ///  Если к моменту вызова не была завершена загрузка предыдущей страницы, то таск будет отменен.
        /// </summary>
        void TryCancelPageFinishedTask()
        {
            _pageFinishedSync_TaskCompletionSource?.TrySetCanceled();
            _pageFinishedSync_TaskCompletionSource = null;
        }

        #endregion
    }
}