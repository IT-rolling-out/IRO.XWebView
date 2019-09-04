using System;
using System.Threading.Tasks;
using IRO.XWebView.Core;
using Newtonsoft.Json;

namespace IRO.CmdLine.OnXWebView
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
            return await XWebView.ExJs<string>(script);
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
Term.setTextColor({serialized});
";

            await XWebView.ExJs<object>(script);
            _savedTextColor = color;
        }

        public string GetSavedTextColor()
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

        public string GetSavedBackgroundColor()
        {
            return _savedBackgroundColor;
        }

        public async Task LoadTerminalIfNotLoaded()
        {
            var script = @"
if(window.Term){
  return true;
}
return false;
";
            await XWebView.AttachBridge();
            var isLoaded = await XWebView.ExJs<bool>(script);
            if (!isLoaded)
            {
                await LoadTerminal();
                await XWebView.AttachBridge();
            }
        }

        async Task LoadTerminal()
        {
            var html = await TerminalJsExtensions.GetTerminalHtml();
            await XWebView.LoadHtml(html);
        }
    }
}
