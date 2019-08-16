using System;
using IRO.Tests.XWebView.Core;

namespace IRO.Tests.XWebView.ChromelyCefGlue
{
    public class ConsoleTestingEnvironment:ITestingEnvironment
    {
        public void Message(string str)
        {
            var defaultColor=Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(str);
            Console.ForegroundColor = defaultColor;
            Console.ReadLine();
        }

        public void Error(string str)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            Console.ForegroundColor = defaultColor;
            Console.ReadLine();
        }
    }
}