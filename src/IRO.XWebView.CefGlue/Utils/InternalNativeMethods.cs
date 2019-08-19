using System;
using System.Runtime.InteropServices;

namespace IRO.XWebView.CefGlue.Utils
{
    public class InternalNativeMethods
    {
        /// <summary>
        /// The gtk lib.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Local
        public const string GtkLib = "libgtk-x11-2.0.so.0";

        /// <summary>
        /// The g obj lib.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Local
        public const string GObjLib = "libgobject-2.0.so.0";

        /// <summary>
        /// The gdk lib.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Local
        public const string GdkLib = "libgdk-x11-2.0.so.0";

        /// <summary>
        /// The glib lib.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        public const string GlibLib = "libglib-2.0.so.0";

        [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, ExactSpelling = false)]
        public static extern void gtk_widget_hide(IntPtr window);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hwnd, ShowWindowCommands nCmdShow);
    }
}
