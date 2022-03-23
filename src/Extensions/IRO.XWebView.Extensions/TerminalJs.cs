using System;
using System.Threading.Tasks;
using IRO.XWebView.Core;
using Newtonsoft.Json;
using IRO.XWebView.Extensions;

namespace IRO.XWebView.Extensions
{
    public class TerminalJs
    {
        public IXWebView XWebView { get; }

        string _savedTextColor = "#ffffff";
        string _savedBackgroundColor = "#000000";

        public TerminalJs(IXWebView xWebView)
        {
            XWebView = xWebView ?? throw new ArgumentNullException(nameof(xWebView));
        }

        public async Task<string> ReadLine()
        {
            await LoadTerminalIfNotLoaded();
            var script = @"
return ReadLine(Term);
";
           
            var res= await XWebView.ExJs<string>(script);
            return res;
        }


        public async Task WriteLine(string str)
        {
            await LoadTerminalIfNotLoaded();
            var serializedStr = JsonConvert.SerializeObject(str);
            var script = $@"
Term.print({serializedStr});
";
            await XWebView.ExJs<object>(script);
        }

        public async Task SetTextColor(string color = "#ffffff")
        {
            await LoadTerminalIfNotLoaded();
            var serialized = JsonConvert.SerializeObject(color);
            var script = $@"
SetTextColor({serialized});
";

            await XWebView.ExJs<object>(script);
            _savedTextColor = color;
        }

        public string GetTextColor()
        {
            return _savedTextColor;
        }

        public async Task SetBackgroundColor(string color = "#ffffff")
        {
            await LoadTerminalIfNotLoaded();
            var serialized = JsonConvert.SerializeObject(color);
            var script = $@"
Term.setBackgroundColor(({serialized});
";

            await XWebView.ExJs<object>(script);
            _savedBackgroundColor = color;
        }

        public string GetBackgroundColor()
        {
            return _savedBackgroundColor;
        }

        async Task LoadTerminalIfNotLoaded()
        {
            var script = @"
if(window.Term){
  return true;
}
return false;
";
            var isLoaded = await XWebView.ExJs<bool>(script);
            if (!isLoaded)
            {
                await LoadTerminal();
            }
        }

        async Task LoadTerminal()
        {
            var html = await TerminalJsExtensions.GetTerminalHtml();
            await XWebView.LoadHtml(html);
            await XWebView.AttachBridge();
            await WriteLine("Terminal initialized!");
        }
    }
}
