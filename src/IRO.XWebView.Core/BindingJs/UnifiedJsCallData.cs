using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IRO.XWebView.Core.BindingJs
{
    public class UnifiedJsCallData
    {
        public JToken Parameters { get; set; }

        public string MethodName { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static UnifiedJsCallData FromJson(string json)
        {
            return JsonConvert.DeserializeObject<UnifiedJsCallData>(json);
        }
    }
}