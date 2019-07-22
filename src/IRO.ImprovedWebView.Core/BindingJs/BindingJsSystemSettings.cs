namespace IRO.ImprovedWebView.Core.BindingJs
{
    public struct BindingJsSystemSettings
    {
        /// <summary>
        /// Method name, that js scripts of <see cref="BindingJsSystem"/> will call.
        /// You must implement it in your 'native brifge' and call following method from
        /// <see cref="BindingJsSystem"/> to make it work.
        /// </summary>
        public string OnJsCallNativeAsyncFunctionName { get; set; }

        /// <summary>
        /// Method name, that js scripts of <see cref="BindingJsSystem"/> will call.
        /// You must implement it in your 'native brifge' and call following method from
        /// <see cref="BindingJsSystem"/> to make it work.
        /// </summary>
        public string OnJsCallNativeSyncFunctionName { get; set; }

        /// <summary>
        /// Method name, that js scripts of <see cref="BindingJsSystem"/> will call.
        /// You must implement it in your 'native brifge' and call following method from
        /// <see cref="BindingJsSystem"/> to make it work.
        /// </summary>
        public string OnJsPromiseFinishedFunctionName { get; set; }

        /// <summary>
        /// Method name, that js scripts of <see cref="BindingJsSystem"/> will call.
        /// You must implement it in your 'native brifge' and call following method from
        /// <see cref="BindingJsSystem"/> to make it work.
        /// </summary>
        public string GetAttachBridgeScriptFunctionName { get; set; }
    }
}