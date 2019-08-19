using System;
using System.Threading.Tasks;
using Chromely.Dialogs;
using IRO.Tests.XWebView.Core;

namespace IRO.Tests.XWebView.ChromelyCefGlue
{
    public class ConsoleTestingEnvironment : ITestingEnvironment
    {
        public void Message(string str)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\n");
            Console.WriteLine(str);
            Console.WriteLine("\n\n");
            Console.ForegroundColor = defaultColor;
            Task.Run(() =>
            {
                ChromelyDialogs.MessageBox(str, new DialogOptions {Title = "----MESSAGE----"});
            });
        }

        public void Error(string str)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\n");
            Console.WriteLine(str);
            Console.WriteLine("\n\n");
            Console.ForegroundColor = defaultColor;
            ChromelyDialogs.MessageBox(str, new DialogOptions { Title = "----ERROR----" });
        }
    }
}