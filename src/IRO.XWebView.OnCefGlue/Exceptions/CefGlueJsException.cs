using System;
using System.Collections.Generic;
using System.Text;
using IRO.XWebView.Core.Exceptions;
using Xilium.CefGlue;

namespace IRO.XWebView.OnCefGlue.Exceptions
{
    public class CefGlueJsException : XWebViewException
    {
        public int EndColumn { get; }

        public int EndPosition { get; }

        public int StartColumn { get; }

        public int StartPosition { get; }

        public int LineNumber { get; }

        public string SourceLine { get; }

        public string ScriptResourceName { get; }

        public string JsMessage { get; }

        public CefGlueJsException(CefV8Exception cefV8Exception):base(cefV8Exception.Message)
        {
            EndColumn = cefV8Exception.EndColumn;
            EndPosition = cefV8Exception.EndPosition;
            StartColumn = cefV8Exception.StartColumn;
            StartPosition = cefV8Exception.StartPosition;
            LineNumber = cefV8Exception.LineNumber;
            SourceLine = cefV8Exception.SourceLine;
            ScriptResourceName = cefV8Exception.ScriptResourceName;
            JsMessage = cefV8Exception.Message;
            
        }
    }
}
