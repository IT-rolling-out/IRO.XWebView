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

        bool _lastLoadWasHandled = false;

        LoadFinishedDelegate _pageFinishedSync_EventHandler;

        TaskCompletionSource<LoadFinishedEventArgs> _pageFinishedSync_TaskCompletionSource;
        #endregion

        public abstract string BrowserType { get; }

        public string Url { get; private set; } = "about:blank";

        /// <summary>
        /// Mean that browser load page or execute js.
        /// </summary>
        public bool IsBusy { get; private set; }

        public abstract ImprovedWebViewVisibility Visibility { get; set; }

        protected BaseImprovedWebView()
        {
            LoadStarted += (s, a) =>
            {
                IsBusy = true;
            };
            LoadFinished += (s, a) =>
            {
                Url = a.Url;
                IsBusy = false;
            };
        }

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

        public async Task WaitWhileBusy()
        {
            await CreateLoadFinishedTask(() => { });
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
        public virtual async Task<TResult> CallCmd<TResult>(string cmdName, object[] parameters = null)
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
            if (_pageFinishedSync_EventHandler != null)
                LoadFinished -= _pageFinishedSync_EventHandler;
            _pageFinishedSync_TaskCompletionSource?.TrySetCanceled();
            _pageFinishedSync_EventHandler = null;
            _pageFinishedSync_TaskCompletionSource = null;
        }

        Task<LoadFinishedEventArgs> CreateLoadFinishedTask(Action act)
        {
            //Локкер нужен для того, чтоб обязательно вернуть нужный таск из метода, даже если он сразу будет отменен.
            lock (_pageFinishedSync_Locker)
            {
                TryCancelPageFinishedTask();
                _pageFinishedSync_TaskCompletionSource = new TaskCompletionSource<LoadFinishedEventArgs>();
                _pageFinishedSync_EventHandler = async (s, a) =>
                {
                    _pageFinishedSync_TaskCompletionSource.TrySetResult(a);
                };
                LoadFinished += _pageFinishedSync_EventHandler;
                act();
                return _pageFinishedSync_TaskCompletionSource.Task;
            }
        }
    }
}