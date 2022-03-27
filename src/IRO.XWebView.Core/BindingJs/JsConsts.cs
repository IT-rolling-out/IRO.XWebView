using IRO.Common.Text;

namespace IRO.XWebView.Core.BindingJs
{
    /// <summary>
    /// Js method names generated randomly on app start. You can get them here.
    /// </summary>
    public static class JsConsts
    {
        /// <summary>
        /// Main native bridge object.
        /// </summary>
        public static readonly string BridgeObj;

        public static readonly string OnJsPromiseFinished = "OnJsPromiseFinished";

        public static readonly string OnJsCall = "OnJsCall";

        public static readonly string NativeBridgeInitialized;

        public static readonly string NativeBridgeInitStarted;

        public static readonly string FullBridgeInit;

        static JsConsts()
        {
            BridgeObj = "o_" + TextExtensions.Generate(5);
            NativeBridgeInitialized = "o_" + TextExtensions.Generate(5);
            NativeBridgeInitStarted = "o_" + TextExtensions.Generate(5);
            FullBridgeInit = "o_" + TextExtensions.Generate(5);
        }
    }
}