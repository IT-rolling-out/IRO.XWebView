using System;
using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Extensions;
using CC = System.ConsoleColor;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestTerminal : BaseXWebViewTest
    {
        protected override async Task RunTest()
        {
            var xwv = await XWVProvider.Resolve(XWebViewVisibility.Visible);
            xwv.Disposing += delegate { ShowMessage($"XWebView disposed."); };

            var terminal = xwv.TerminalJs();
            await terminal.WriteLine("Here we are testing terminal.");
            await terminal.WriteLine("Send me text and i will send it back with random color.");
            await terminal.SetTextColor(CC.DarkRed.ToHex());
            await terminal.WriteLine("Send me comman 'q' to exit.");
            await terminal.SetTextColor();

            var colorsArray = Enum.GetValues(typeof(CC));
            var random = new Random();

            while (true)
            {
                var str = await terminal.ReadLine();
                if (str.Trim() == "q")
                {
                    break;
                }

                var randomColor = (CC)colorsArray.GetValue(
                  random.Next(colorsArray.Length)
                  );
                await terminal.SetTextColor(randomColor.ToHex());
                await terminal.WriteLine("You send me: \n"+ str);
                await terminal.SetTextColor();
            }
            xwv.Dispose();
        }
    }
}