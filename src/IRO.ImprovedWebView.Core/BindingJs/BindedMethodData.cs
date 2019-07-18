using System.Reflection;

namespace IRO.ImprovedWebView.Core.BindingJs
{
    class BindedMethodData
    {
        public object InvokeOn { get; set; }

        public string JsObjectName { get; set; }

        public string FunctionName { get; set; }

        public MethodInfo Method { get; set; }

        public ParameterInfo[] Parameters { get; set; }
    }
}