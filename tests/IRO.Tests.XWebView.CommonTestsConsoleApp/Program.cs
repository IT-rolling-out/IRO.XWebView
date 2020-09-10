using System;
using System.Reflection;
using System.Threading.Tasks;
using IRO.XWebView.Core.BindingJs;

namespace IRO.Tests.XWebView.CommonTestsConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TestBindingJsSystemScriptGen();
            Console.ReadLine();
        }

        static void TestBindingJsSystemScriptGen()
        {
            var bs = new BindingJsSystem();
            var t = Task.Run(() => { });
            var methods = t
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var mi in methods)
            {
                bs.BindToJs(mi, t, mi.Name, "TaskMethods");
            }

            var str = bs.GetAttachBridgeScript();
            Console.WriteLine(str);
        }
    }
}