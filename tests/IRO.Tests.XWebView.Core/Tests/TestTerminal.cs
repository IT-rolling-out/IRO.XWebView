using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using IRO.CmdLine;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.CmdLine.OnXWebView;
using IRO.Storage.DefaultStorages;

namespace IRO.Tests.XWebView.Core.Tests
{
    public class TestTerminal : IXWebViewTest
    {
        public async Task RunTest(IXWebViewProvider xwvProvider, ITestingEnvironment env, TestAppSetupConfigs appConfigs)
        {

            var xwv = await xwvProvider.Resolve(XWebViewVisibility.Visible);
            xwv.Disposing += delegate { env.Message($"XWebView disposed."); };
            await xwv.TerminalJs().LoadTerminalIfNotLoaded();
            var consoleHandler = new TerminalJsConsoleHandler(xwv);
            var keyValueStorage = new FileStorage("storage", appConfigs.ContentPath);
            var cmdLineExtensions = new CmdLineExtension(consoleHandler, keyValueStorage);
            var cmds = new CmdSwitcher();
            cmds.PushCmdInStack(new CmdLineFacade(cmdLineExtensions));
            //Note that this method block current thread and can't be used from ui thread.
            //Enter 'q' to exit.
            cmds.RunDefault();
            xwv.Dispose();
        }

        public class CmdLineFacade : CommandLineBase
        {
            public CmdLineFacade(CmdLineExtension cmdLineExtension = null) : base(cmdLineExtension)
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
