using System;
using System.Threading;
using System.Threading.Tasks;
using IRO.XWebView.Core;

namespace IRO.CmdLine.OnXWebView
{
    public class TerminalJsConsoleHandler : IConsoleHandler
    {
        public IXWebView XWebView { get; }

        readonly TerminalJs _terminalJs;

        public TerminalJsConsoleHandler(IXWebView xWebView)
        {
            XWebView = xWebView ?? throw new ArgumentNullException(nameof(xWebView));
            _terminalJs = XWebView.TerminalJs();
        }

        public string ReadJson(string jsonPrototypeString)
        {
            return SharedConsoleMethods.ReadJson(jsonPrototypeString, this);
        }

        public void WriteLine()
        {
            WriteLine("", null);
        }

        public string ReadLine()
        {
            return _terminalJs.ReadLine().Result;
        }

        public void Write(string str, ConsoleColor? consoleColor)
        {
            WriteLine(str, consoleColor);
        }

        public void WriteLine(string str, ConsoleColor? consoleColor)
        {
            WriteLineAsync(str, consoleColor).Wait();
        }

        public async Task WriteLineAsync(string str, ConsoleColor? consoleColor)
        {
            consoleColor ??= ConsoleColor.White;
            var currentColor = _terminalJs.GetSavedTextColor();
            var color = TerminalJsExtensions.ConsoleColorToHex(consoleColor.Value);
            await _terminalJs.SetTextColor(color);
            await _terminalJs.WriteLine(str);
            await _terminalJs.SetTextColor(currentColor);
        }
    }
}