using System;
using IRO.Tests.XWebView.Core;

namespace IRO.Tests.XWebView.CefSharpOffScreen
{
    public class ConsoleTestingEnvironment : ITestingEnvironment
    {
        public void Message(string str)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\n");
            Console.WriteLine("MESSAGE: " + str);
            Console.WriteLine("\n\n");
            Console.ForegroundColor = defaultColor;

        }

        public void Error(string str)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\n");
            Console.WriteLine("ERROR: " + str);
            Console.WriteLine("\n\n");
            Console.ForegroundColor = defaultColor;
            Console.ReadLine();
        }
    }
}