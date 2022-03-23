using IRO.Common.EmbeddedResources;
using IRO.Tests.XWebView.Core.JsInterfaces;
using IRO.XWebView.Core;
using IRO.XWebView.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace IRO.Tests.XWebView.Core
{
    public class TestApp
    {
        public string MainMenuPagePath { get; private set; }

        public async Task Setup(TestAppSetupConfigs configs)
        {

            var mainXWV = configs.MainXWebView;
            var provider = configs.Provider;
            var env = configs.TestingEnvironment;
            var contentPath = configs.ContentPath ?? AppDomain.CurrentDomain.BaseDirectory;

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = (Exception)e.ExceptionObject;
                var msg = "App unhandled error!\n" + ex.ToString();
                Debug.WriteLine(msg);
                env.Error(msg);
            };

            var nativeJsInterface = new NativeJsInterface(
                configs
                );
            //Now this object will be accessible in main webview on each page, after you call AttachBridge.
            mainXWV.BindToJs(nativeJsInterface, "Native");

            //Automatically AttachBridge not implemented due WebViews limitation and perfomance.
            //See workarounds on github. You can use code below to attach bridge on each page load, but this method will be async.
            //So there no guarantees that bridge attach will be finished at the right time even when you use 'await xwv.LoadUrl()'.
            mainXWV.LoadFinished += async delegate
            {
                try
                {
                    await mainXWV.IncludePolyfill();
                    await mainXWV.AttachBridge();
                    //Notify page that bridge attached. Define this on your page to do some things.
                    var script = @"
try{
  if(!window.IsBridgeAttachedInvoked){
    window.IsBridgeAttachedInvoked=true;
    BridgeAttached();
  }
}catch(e){}";
                    await mainXWV.ExJs<object>(script);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"LoadFinished event handler exception '{ex}'.");
                }
            };


            //In current project i use 'BuildAction:Embedded Resource' to include all needed file,
            //because not all platforms (Xamarin) support 'BuildAction:Content'. You can put your files in xamarin projects assets,
            //but i decide to put them in this assembly and extract on launch.
            var extractResourcesPath = Path.Combine(contentPath, "WebAppSource");

            //If you will use this method on production i recomend to check somehow if files of current app version is extracted.
            //In this code they always re-extracted on app launch.
            var embeddedDirPath = "IRO.Tests.XWebView.Core.WebAppSource";
            var assembly = Assembly.GetExecutingAssembly();
            //From IRO.EmbeddedResourcesHelpers nuget.
            assembly.ExtractEmbeddedResourcesDirectory(embeddedDirPath, extractResourcesPath);

            await mainXWV.WaitWhileNavigating();
            MainMenuPagePath = "file://" + extractResourcesPath + "/MainPage.html";
            await mainXWV.LoadUrl(MainMenuPagePath);
        }
    }
}
