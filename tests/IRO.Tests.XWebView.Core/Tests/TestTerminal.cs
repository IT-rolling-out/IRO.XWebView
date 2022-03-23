using System;
using System.Threading.Tasks;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Extensions;
using CC = System.ConsoleColor;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestTerminal : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env, TestAppSetupConfigs appConfigs)
        {
            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Visible);
            xwv.Disposing += delegate { env.Message($"XWebView disposed."); };

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


                await terminal.SetTextColor();
                await terminal.WriteLine("You send me: ");
                var randomColor = (CC)colorsArray.GetValue(
                    random.Next(colorsArray.Length)
                    );
                await terminal.SetTextColor(randomColor.ToHex());
                await terminal.WriteLine("    " + str);
            }
            xwv.Dispose();
        }
    }
}