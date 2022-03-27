using CefSharp.JavascriptBinding;
using System.Linq;
using System.Reflection;


namespace IRO.XWebView.CefSharp
{
    /// <summary>
    /// This source code taken from here https://github.com/cefsharp/CefSharp/blob/4ef267cabf65cc84753afc1f7bd7be0bfdf65dd1/CefSharp/JavascriptBinding/CamelCaseJavascriptNameConverter.cs
    /// <para></para>
    /// Difference is that here it return PascalCaseName, not camelCaseName
    /// </summary>
    internal class PascalCaseJavascriptNameConverter : IJavascriptNameConverter
    {
        string IJavascriptNameConverter.ConvertToJavascript(MemberInfo memberInfo) 
            => ConvertToJavascript(memberInfo);

        /// <summary>
        /// Get the javascript name for the property/field/method.
        /// Typically this would be based on <see cref="MemberInfo.Name"/>
        /// </summary>
        /// <param name="memberInfo">property/field/method</param>
        /// <returns>javascript name</returns>
        protected virtual string ConvertToJavascript(MemberInfo memberInfo) => ConvertMemberInfoNameToCamelCase(memberInfo);

        string IJavascriptNameConverter.ConvertReturnedObjectPropertyAndFieldToNameJavascript(MemberInfo memberInfo) 
            => ConvertReturnedObjectPropertyAndFieldToNameJavascript(memberInfo);

        /// <summary>
        /// This method exists for backwards compatability reasons, historically
        /// only the bound methods/fields/properties were converted. Objects returned
        /// from a method call were not translated. To preserve this functionality
        /// for upgrading users we split this into two methods. Typically thie method
        /// would return the same result as <see cref="ConvertToJavascript(MemberInfo)"/>
        /// Issue #2442
        /// </summary>
        /// <param name="memberInfo">property/field/method</param>
        /// <returns>javascript name</returns>
        protected virtual string ConvertReturnedObjectPropertyAndFieldToNameJavascript(MemberInfo memberInfo) 
            => ConvertMemberInfoNameToCamelCase(memberInfo);

        /// <summary>
        /// Converts the <see cref="MemberInfo.Name"/> to CamelCase
        /// </summary>
        /// <param name="memberInfo">memberInfo</param>
        /// <returns>camelcased name</returns>
        protected static string ConvertMemberInfoNameToCamelCase(MemberInfo memberInfo)
        {
            var name = memberInfo.Name;

            // camelCase says that if the string is only one character that it is preserved.
            if (name.Length == 1)
            {
                return name;
            }

            // camelCase says that if the entire string is uppercase to preserve it.
            //TODO: We need to cache these values to avoid the cost of validating this
            if (name.All(char.IsUpper))
            {
                return name;
            }

            //USED THIS
            return name;

            //OVER THIS
            //var firstHalf = name.Substring(0, 1);
            //var remainingHalf = name.Substring(1);

            //return firstHalf.ToLowerInvariant() + remainingHalf;
        }
    }
}

