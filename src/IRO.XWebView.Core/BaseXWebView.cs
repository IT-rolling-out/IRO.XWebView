using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IRO.Threading;
using IRO.XWebView.Core.BindingJs;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Events;
using IRO.XWebView.Core.Exceptions;
using IRO.XWebView.Core.Models;
using IRO.XWebView.Core.Utils;
using Newtonsoft.Json;

namespace IRO.XWebView.Core
{
    public abstract class BaseXWebView : IXWebView
    {
        static readonly Random Rand = new Random();

        bool _isNavigating;
        string _url;

        protected BaseXWebView(IBindingJsSystem bindingJsSystem = null)
        {
            ThreadSync = XWebViewThreadSync.Inst;
            BindingJsSystem = bindingJsSystem ?? new BindingJsSystem();
            Id = Rand.Next(99999999);
            LoadStarted += async (s, a) =>
            {
                _isNavigating = true;
                //if (AutomaticallyAttachBridge)
                //{
                //    await this.AttachBridge();
                //}
            };
            LoadFinished += async (s, a) =>
            {
                if (AutomaticallyAttachBridge)
                {
                    await this.AttachBridge();
                }
                _url = a.Url;
                _isNavigating = false;
            };
        }

        public int Id { get; }

        protected IBindingJsSystem BindingJsSystem { get; }

        /// <summary>
        /// False by default. If true - will add your native methods support on all pages after they uploaded.
        /// </summary>
        public bool AutomaticallyAttachBridge { get; set; }

        /// <summary>
        /// Base version use backing field to set, so you can override it.
        /// </summary>
        public virtual string Url => _url;

        /// <summary>
        /// Base version use backing field to set, so you can override it.
        /// </summary>
        public virtual bool IsNavigating => _isNavigating;

        public abstract string BrowserName { get; }

        public bool UnsafeEval
        {
            get => BindingJsSystem.UnsafeEval;
            set => BindingJsSystem.UnsafeEval = value;
        }

        public IDictionary<string, object> Data { get; } = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// By default here used <see cref="XWebViewThreadSync.Inst"/> . If you library developer and your webview
        /// allow to use separate thread for each instance of webview - you can use defferent <see cref="ThreadSync"/>
        /// for each instance. Just override this property of <see cref="BaseXWebView"/>.
        /// </summary>
        public virtual ThreadSyncContext ThreadSync { get; }

        public virtual async Task<LoadResult> LoadUrl(string url)
        {
            ThrowIfDisposed();
            var finishedEventArgs = await CreateLoadFinishedTask(() => { StartLoading(url); });
            return new LoadResult(finishedEventArgs.Url);
        }

        public virtual async Task<LoadResult> LoadHtml(string html, string baseUrl = "about:blank")
        {
            ThrowIfDisposed();
            var finishedEventArgs = await CreateLoadFinishedTask(() => { StartLoadingHtml(html, baseUrl); });
            return new LoadResult(finishedEventArgs.Url);
        }

        public virtual async Task<LoadResult> Reload()
        {
            ThrowIfDisposed();
            var finishedEventArgs = await CreateLoadFinishedTask(
                StartReloading
            );
            return new LoadResult(finishedEventArgs.Url);
        }

        public virtual async Task<LoadResult> GoForward()
        {
            ThrowIfDisposed();
            var canGoForward = CanGoForward();
            var args = new GoForwardEventArgs();
            args.CanGoForward = canGoForward;
            await OnGoForwardRequested(args);
            if (args.Cancel)
                throw new TaskCanceledException("Go forward cancelled.");
            var finishedEventArgs = await CreateLoadFinishedTask(StartGoForward);
            return new LoadResult(finishedEventArgs.Url);
        }

        public virtual async Task<LoadResult> GoBack()
        {
            ThrowIfDisposed();
            var canGoBack = CanGoBack();
            var args = new GoBackEventArgs();
            args.CanGoBack = canGoBack;
            await OnGoBackRequested(args);
            if (args.Cancel)
                throw new TaskCanceledException("Go back cancelled.");
            var finishedEventArgs = await CreateLoadFinishedTask(StartGoBack);
            return new LoadResult(finishedEventArgs.Url);
        }

        public virtual async Task AttachBridge()
        {
            ThrowIfDisposed();
            var script = BindingJsSystem.GetAttachBridgeScript();
            UnmanagedExecuteJavascriptAsync(script);
        }

        public virtual async Task WaitWhileNavigating()
        {
            ThrowIfDisposed();
            if (!IsNavigating)
                return;
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            LoadFinishedDelegate ev = null;
            ev = async (s, e) =>
            {
                LoadFinished -= ev;
                tcs.TrySetResult(null);
            };
            LoadFinished += ev;
            if (IsNavigating)
            {
                await tcs.Task;
            }
        }

        public void BindToJs(MethodInfo methodInfo, object invokeOn, string functionName, string jsObjectName)
        {
            ThrowIfDisposed();
            BindingJsSystem.BindToJs(methodInfo, invokeOn, functionName, jsObjectName);
        }

        public void UnbindFromJs(string functionName, string jsObjectName)
        {
            ThrowIfDisposed();
            BindingJsSystem.UnbindFromJs(functionName, jsObjectName);
        }

        public void UnbindAllFromJs()
        {
            ThrowIfDisposed();
            BindingJsSystem.UnbindAllFromJs();
        }

        public virtual async Task<TResult> ExJs<TResult>(string script, bool promiseResultSupport = true,
            int? timeoutMS = null)
        {
            return await BindingJsSystem.ExJs<TResult>(this, script, promiseResultSupport, timeoutMS);
        }

        public virtual async Task<string> GetHtml()
        {
            var script = @"
return document.documentElement.outerHTML;
";
            return await ExJs<string>(script);
        }

        public abstract void Stop();

        public abstract bool CanGoForward();

        public abstract bool CanGoBack();

        public abstract object Native();

        public abstract void ClearCookies();

        /// <summary>
        /// Execute your script in browser without any manipulations.
        /// Doesn't support promises.
        /// <para></para>
        /// Used as base method to build javascript wrapper for sync scripts.
        /// </summary>
        public abstract Task<string> UnmanagedExecuteJavascriptWithResult(string script, int? timeoutMS = null);

        /// <summary>
        /// Execute your script in browser without any manipulations.
        /// Doesn't support promises.
        /// <para></para>
        /// /// Used as base method to build javascript wrapper for async scripts (callbacks).
        /// </summary>
        public abstract void UnmanagedExecuteJavascriptAsync(string script, int? timeoutMS = null);

        protected virtual void StartGoForward()
        {
            var script = "window.history.forward();";
            UnmanagedExecuteJavascriptAsync(script);
        }

        protected virtual void StartGoBack()
        {
            var script = "window.history.back();";
            UnmanagedExecuteJavascriptAsync(script);
        }

        protected virtual void StartReloading()
        {
            var script = "document.location.reload(true);";
            UnmanagedExecuteJavascriptAsync(script);
        }

        protected virtual void StartLoading(string url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            var urlSerialized = JsonConvert.SerializeObject(url);
            var script = $@"
document.location.href={urlSerialized};
";
            UnmanagedExecuteJavascriptAsync(script);
        }

        /// <summary>
        /// Base implemention use crunches, but it is crossplatform.
        /// Better if you implement native metod.
        /// <para></para>
        /// Not tested.
        /// </summary>
        protected virtual void StartLoadingHtml(string data, string baseUrl)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            LoadFinishedDelegate evHandler = null;
            evHandler =async (s, e) =>
              {
                  LoadFinished -= evHandler;
                  try
                  {
                      if (e.IsError)
                      {
                          tcs?.TrySetException(
                            new XWebViewException($"Error in {nameof(StartLoadingHtml)} '{e.ErrorDescription}'.")
                            );
                      }
                      else
                      {
                          var serializedHtml = JsonConvert.SerializeObject(data);
                          UnmanagedExecuteJavascriptAsync($"document.write({serializedHtml})");
                          tcs?.TrySetResult(null);
                      }
                  }
                  finally
                  {
                      tcs = null;
                  }
              };
            LoadFinished += evHandler;
            StartLoading(baseUrl);
            tcs.Task.Wait();
        }

        #region Initialize part.
        public bool IsInitialized { get; private set; }

        public event EventHandler Initialized;

        protected void SetInitialized()
        {
            if (IsInitialized)
                return;
            IsInitialized = true;
            Initialized?.Invoke(this, new EventArgs());
        }
        #endregion

        #region Visibility.

        public abstract bool CanSetVisibility { get; }

        XWebViewVisibility _visibility;

        public XWebViewVisibility Visibility
        {
            get => GetVisibilityState();
            set
            {
                _visibility = value;
                if (!CanSetVisibility)
                {
                    throw new XWebViewException($"Can't change visibility of {GetType().Name}.");
                }

                SetVisibilityState(Visibility);
            }
        }

        protected abstract void SetVisibilityState(XWebViewVisibility visibility);

        protected abstract XWebViewVisibility GetVisibilityState();
        #endregion

        #region Events.

        public event GoBackDelegate GoBackRequested;

        public event GoForwardDelegate GoForwardRequested;

        public event LoadStartedDelegate LoadStarted;

        public event LoadFinishedDelegate LoadFinished;

        const bool SuppressEventsErrors = false;

        protected async Task OnGoBackRequested(GoBackEventArgs args)
        {
            await GoBackRequested.InvokeOneByOne(SuppressEventsErrors, this, args);
        }

        protected async Task OnGoForwardRequested(GoForwardEventArgs args)
        {
            await GoForwardRequested.InvokeOneByOne(SuppressEventsErrors, this, args);
        }

        protected internal async Task OnLoadStarted(LoadStartedEventArgs args)
        {
            await LoadStarted.InvokeOneByOne(SuppressEventsErrors, this, args);
        }

        protected internal async Task OnLoadFinished(LoadFinishedEventArgs args)
        {
            await LoadFinished.InvokeOneByOne(SuppressEventsErrors, this, args);
        }

        #endregion

        #region Load finished sync part.

        SemaphoreSlim _loadFinishedTask_SemaphoreSlim = new SemaphoreSlim(1, 1);

        TaskCompletionSource<LoadFinishedEventArgs> _pageFinishedSync_TaskCompletionSource;

        /// <summary>
        ///  Execute pased action and return first page load result.
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        async Task<LoadFinishedEventArgs> CreateLoadFinishedTask(Action act)
        {
            var tcs = new TaskCompletionSource<LoadFinishedEventArgs>(
                TaskContinuationOptions.RunContinuationsAsynchronously
                );
            await _loadFinishedTask_SemaphoreSlim.WaitAsync();
            try
            {
                TryCancelPageFinishedTask();
                Stop();
                await WaitWhileNavigating();

                _pageFinishedSync_TaskCompletionSource = tcs;
                LoadFinishedDelegate loadFinishedHandler = null;
                loadFinishedHandler =async (s, a) =>
                {
                    LoadFinished -= loadFinishedHandler;
                    if (a.IsError)
                    {
                        Debug.WriteLine("XWebView error: 'load exception'.");
                        tcs?.TrySetException(
                            new XWebViewException($"Load exception: {a.ErrorDescription} .")
                            );
                    }
                    else if (a.WasCancelled)
                    {
                        tcs?.TrySetException(new TaskCanceledException("Load was cancelled."));
                    }
                    else
                    {
                        tcs?.TrySetResult(a);
                    }
                };
                LoadFinished += loadFinishedHandler;
                act();
            }
            finally
            {
                _loadFinishedTask_SemaphoreSlim.Release();
            }
            return await tcs.Task;
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

        #region Disposing.

        public bool IsDisposed { get; private set; }

        public event Action<object, EventArgs> Disposing;

        public virtual void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            Disposing?.Invoke(this, EventArgs.Empty);
        }

        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion
    }
}