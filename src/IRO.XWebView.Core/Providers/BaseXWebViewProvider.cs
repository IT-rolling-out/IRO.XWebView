using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;

namespace IRO.XWebView.Core.Providers
{
    public abstract class BaseXWebViewProvider : IXWebViewProvider
    {
        readonly HashSet<IXWebView> _providedXWebView = new HashSet<IXWebView>();

        public IReadOnlyCollection<IXWebView> ProvidedXWebView => _providedXWebView;

        public void DisposeProvidedXWebView()
        {
            var list = _providedXWebView.ToList();
            var exList = new List<Exception>();
            foreach (var xwv in list)
            {
                try
                {
                    xwv.Dispose();
                }
                catch (Exception ex)
                {
                    exList.Add(ex);
                }
            }

            if (exList.Count == 1)
            {
                throw exList[0];
            }
            else if (exList.Count > 1)
            {
                throw new AggregateException(exList);
            }
        }

        public async Task<IXWebView> Resolve(XWebViewVisibility preferredVisibility = XWebViewVisibility.Hidden)
        {
            var xwv = await ProtectedResolve(preferredVisibility);
            if (!_providedXWebView.Contains(xwv))
            {
                _providedXWebView.Add(xwv);
                xwv.Disposing += delegate
                {
                    try
                    {
                        _providedXWebView.Remove(xwv);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                };
            }
            return xwv;
        }

        protected abstract Task<IXWebView> ProtectedResolve(XWebViewVisibility preferredVisibility);
    }
}