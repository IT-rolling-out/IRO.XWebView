using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using IRO.ImprovedWebView.Core.BindingJs;
using IRO.ImprovedWebView.Core.EventsAndDelegates;

namespace IRO.ImprovedWebView.Core
{
    public abstract class BaseImprovedWebView : IImprovedWebView
    {
        #region Private data.
        readonly object _pageFinishedSync_Locker = new object();

        TaskCompletionSource<LoadFinishedEventArgs> _pageFinishedSync_TaskCompletionSource;
        #endregion

        public abstract string BrowserType { get; }

        public abstract string Url { get; protected set; }

        public abstract bool IsBusy { get; protected set; }

        public abstract ImprovedWebViewVisibility Visibility { get; set; }

        protected BindingJsSystem BindingJsSystem { get; }

        protected BaseImprovedWebView(BindingJsSystemSettings bindingJsSystemSettings)
        {
            BindingJsSystem = new BindingJsSystem(bindingJsSystemSettings);
        }

        public async Task<LoadFinishedEventArgs> LoadUrl(string url)
        {
            var res = await CreateLoadFinishedTask(() =>
            {
                StartLoading(url);
            });
            return res;
        }

        public async Task<LoadFinishedEventArgs> LoadHtml(string html, string baseUrl = "about:blank")
        {
            var res = await CreateLoadFinishedTask(() =>
            {
                StartLoadingHtml(html, baseUrl);
            });
            return res;
        }

        public virtual async Task<LoadFinishedEventArgs> Reload()
        {
            var res = await CreateLoadFinishedTask(
                StartReloading
                );
            return res;
        }

        public virtual async Task<LoadFinishedEventArgs> GoForward()
        {
            var canGoForward = CanGoForward();
            var args = new GoForwardEventArgs();
            args.CanGoForward = canGoForward;
            OnGoForwardRequested(args);
            if (args.Cancel)
                throw new TaskCanceledException("Go forward cancelled.");
            var res = await CreateLoadFinishedTask(StartGoForward);
            return res;
        }

        public virtual async Task<LoadFinishedEventArgs> GoBack()
        {
            var canGoBack = CanGoBack();
            var args = new GoBackEventArgs();
            args.CanGoBack = canGoBack;
            OnGoBackRequested(args);
            if (args.Cancel)
                throw new TaskCanceledException("Go back cancelled.");
            var res = await CreateLoadFinishedTask(StartGoBack);
            return res;
        }


        public virtual async Task AttachBridge()
        {
            var script = BindingJsSystem.GetAttachBridgeScript();
            await ExJsDirect(script);
        }

        public abstract void Stop();

        public async Task WaitWhileBusy()
        {
            if (!IsBusy)
                return;
            if (_pageFinishedSync_TaskCompletionSource == null)
            {
                //Create new.
                await CreateLoadFinishedTask(() => { });
            }
            else
            {
                //Await previous.
                await _pageFinishedSync_TaskCompletionSource.Task;
            }
        }

        public void BindToJs(MethodInfo methodInfo, object invokeOn, string functionName, string jsObjectName)
        {
            BindingJsSystem.BindToJs(methodInfo, invokeOn, functionName, jsObjectName);
        }

        public void UnbindFromJs(string functionName, string jsObjectName)
        {
            BindingJsSystem.UnbindFromJs(functionName, jsObjectName);
        }

        public virtual async Task<TResult> ExJs<TResult>(string script, bool promiseSupport = false, int? timeoutMS = null)
        {
            return await BindingJsSystem.ExJs<TResult>(this, script, promiseSupport, timeoutMS);
        }

        public abstract Task<string> ExJsDirect(string script, int? timeoutMS = null);

        public abstract bool CanGoForward();

        public abstract bool CanGoBack();

        public abstract object Native();

        public abstract void StartGoForward();

        public abstract void StartGoBack();

        public abstract void StartReloading();

        public abstract void StartLoading(string url);

        public abstract void StartLoadingHtml(string data, string baseUrl);

        public abstract void Dispose();

        /// <summary>
        /// Execute pased action and return first page load result.
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
                        Debug.WriteLine($"ImprovedWebView error: 'load exception'");
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
        ///Вызывается для удаления ссылок на обработчик события и промиса.
        ///Если к моменту вызова не была завершена загрузка предыдущей страницы, то таск будет отменен.
        /// </summary>
        void TryCancelPageFinishedTask()
        {
            _pageFinishedSync_TaskCompletionSource?.TrySetCanceled();
            _pageFinishedSync_TaskCompletionSource = null;
        }

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

    }
}