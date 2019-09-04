using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IRO.CmdLine;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.CmdLine.OnXWebView;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestTerminal : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env, TestAppSetupConfigs appConfigs)
        {

            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Visible);
            await xwv.TerminalJs().LoadTerminalIfNotLoaded();
            await xwv.TerminalJs().SetTextColor();
            var consoleHandler = new TerminalJsConsoleHandler(xwv);
            consoleHandler.WriteLine("Hi, console!", ConsoleColor.Red);
            await Task.Delay(2000);
            var cmds = new CmdSwitcher();
            cmds.PushCmdInStack(new CmdLineFacade(consoleHandler));
            //Note that this method block current thread and can't be used from ui thread.
            //Enter 'q' to exit.
            cmds.RunDefault();
            xwv.Dispose();
        }

        public class CmdLineFacade : CommandLineBase
        {
            public CmdLineFacade(IConsoleHandler consoleHandler = null) : base(consoleHandler)
            {
            }

            [CmdInfo]
            public void Test1()
            {
                //Easy read complex objects with newtonsoft json.
                //Will be opened default text editor with example value.
                var res = ReadResource<Dictionary<object, object>>("test res");
            }

            [CmdInfo(Description = "In current method you can pass parameters.")]
            public void Test2(DateTime dtParam, string strParam, bool boolParam, int intParam)
            {
                //Easy print complex objects with newtonsoft json.
                Cmd.WriteLine(new Dictionary<string, object>()
            {
                {nameof(dtParam), dtParam},
                {nameof(boolParam), boolParam},
                {nameof(strParam),strParam },
                {nameof(intParam),intParam }
            });
            }

            [CmdInfo]
            public void Test3()
            {
                var res = ReadResource<bool>("test simple res");
            }
        }
    }
}
