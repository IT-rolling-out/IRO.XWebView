using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
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

        public async Task<LoadFinishedEventArgs> LoadUrl(string url)
        {
            var res = await CreateLoadFinishedTask(
                () =>
                {
                    StartLoading(url);
                }
                );
            return res;
        }

        public async Task<LoadFinishedEventArgs> LoadHtml(string html, string baseUrl = "about:blank")
        {
            var res = await CreateLoadFinishedTask(
                () =>
                {
                    StartLoadingHtml(html, baseUrl);
                }
                );
            return res;
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
            throw new NotImplementedException();
        }

        public abstract Task<TResult> ExJs<TResult>(string script, int? timeoutMS = null);

        /// <summary>
        /// Override and add any your code.
        /// <para></para>
        /// Not js function call.
        /// Just dynamic function call, that must be processed on browser level.
        /// Works like messaging system.
        /// For example, 'InjectJQuery' cmd.
        /// </summary>
        public virtual Task<TResult> CallCmd<TResult>(string cmdName, object[] parameters = null)
        {
            throw new NotImplementedException();
        }

        public abstract void Finish();

        public abstract Task<bool> CanGoForward();

        public abstract Task GoForward();

        public abstract Task<bool> CanGoBack();

        public abstract Task GoBack();

        public abstract object Native();

        public abstract void StartLoading(string url);

        public abstract void StartLoadingHtml(string data, string baseUrl);

        #region Events.
        public event GoBackDelegate GoBackRequested;

        public event GoForwardDelegate GoForwardRequested;

        public event LoadStartedDelegate LoadStarted;

        public event LoadFinishedDelegate LoadFinished;

        public event Action<object, EventArgs> Finishing;

        public event Action<object, EventArgs> Finished;

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

        protected void OnFinishing()
        {
            Finishing?.Invoke(this, EventArgs.Empty);
        }

        protected void OnFinished()
        {
            Finished?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        /// <summary>
        ///Вызывается для удаления ссылок на обработчик события и промиса.
        ///Если к моменту вызова не была завершена загрузка предыдущей страницы, то таск будет отменен.
        /// </summary>
        void TryCancelPageFinishedTask()
        {
            _pageFinishedSync_TaskCompletionSource?.TrySetCanceled();
            _pageFinishedSync_TaskCompletionSource = null;
        }

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
                        tcs?.TrySetException(
                            new NotImplementedException($"Load exception: {a.ErrorDescription} .")
                            );
                    }
                    else
                    {
                        tcs?.TrySetResult(a);
                    }
                };
                LoadFinished += loadFinishedHandler;
                act();
            }
            return await _pageFinishedSync_TaskCompletionSource.Task;
        }
    }
}