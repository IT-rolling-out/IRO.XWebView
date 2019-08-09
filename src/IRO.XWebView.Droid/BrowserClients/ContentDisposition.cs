using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace IRO.XWebView.Droid.BrowserClients
{
    /// <summary>
    /// Used in files download implemention.
    /// </summary>
    public class ContentDisposition
    {
        static readonly Regex RegexCheck = new Regex(
            "^([^;]+);(?:\\s*([^=]+)=((?<q>\"?)[^\"]*\\k<q>);?)*$",
            RegexOptions.Compiled
        );

        public ContentDisposition(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(nameof(str));
            }

            var match = RegexCheck.Match(str);
            if (!match.Success)
            {
                throw new FormatException("Input is not a valid content-disposition string.");
            }

            var typeGroup = match.Groups[1];
            var nameGroup = match.Groups[2];
            var valueGroup = match.Groups[3];

            var groupCount = match.Groups.Count;
            var paramCount = nameGroup.Captures.Count;

            Type = typeGroup.Value;
            Parameters = new StringDictionary();

            for (var i = 0; i < paramCount; i++)
            {
                var name = nameGroup.Captures[i].Value;
                var value = valueGroup.Captures[i].Value;

                if (name.Equals("filename", StringComparison.InvariantCultureIgnoreCase))
                {
                    FileName = value;
                }
                else
                {
                    Parameters.Add(name, value);
                }
            }
        }

        public string FileName { get; }

        public StringDictionary Parameters { get; }

        public string Type { get; }
    }
}