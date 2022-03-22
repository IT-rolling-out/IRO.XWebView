using IRO.EmbeddedResources;
using IRO.XWebView.Core;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace IRO.CmdLine.OnXWebView
{
    public static class TerminalJsExtensions
    {
        private static string CachedStr;

        public static async Task<string> GetTerminalHtml()
        {
            if (CachedStr == null)
            {

                var name = $"{typeof(TerminalJsExtensions).Namespace}.EmbeddedFiles.terminal.html";
                CachedStr = await EmbeddedResourcesHelpers.ReadEmbeddedResourceText(
                    Assembly.GetExecutingAssembly(),
                    name
                );
            }
            return CachedStr;
        }

        public static TerminalJs TerminalJs(this IXWebView xwv)
        {
            if (xwv.Data.TryGetValue("TerminalJs", out var value))
            {
                return (TerminalJs)value;
            }
            else
            {
                var tjs = new TerminalJs(xwv);
                xwv.Data["TerminalJs"] = tjs;
                return tjs;
            }
        }

        public static string ConsoleColorToHex(ConsoleColor consoleColor)
        {
            switch (consoleColor)
            {
                case ConsoleColor.Black:
                    return "#000000";
                case ConsoleColor.Blue:
                    return "#0000ff";
                case ConsoleColor.Cyan:
                    return "#00FFFF";
                case ConsoleColor.DarkBlue:
                    return "#00008b";
                case ConsoleColor.DarkCyan:
                    return "#008B8B";
                case ConsoleColor.DarkGray:
                    return "#A9A9A9";
                case ConsoleColor.DarkGreen:
                    return "#006400";
                case ConsoleColor.DarkMagenta:
                    return "#8b008b";
                case ConsoleColor.DarkRed:
                    return "#8b0000";
                case ConsoleColor.DarkYellow:
                    return "#CCCC00";
                case ConsoleColor.Gray:
                    return "#808080";
                case ConsoleColor.Green:
                    return "#008000";
                case ConsoleColor.Magenta:
                    return "#FF00FF";
                case ConsoleColor.Red:
                    return "#FF0000";
                case ConsoleColor.White:
                    return "#ffffff";
                case ConsoleColor.Yellow:
                    return "#ffff00";
                default:
                    return "#ffffff";
            }
        }
    }
}