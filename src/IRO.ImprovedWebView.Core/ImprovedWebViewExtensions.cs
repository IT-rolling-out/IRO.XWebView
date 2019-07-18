﻿using System;
using System.Reflection;

namespace IRO.ImprovedWebView.Core
{
    public static class ImprovedWebViewExtensions
    {
        /// <summary>
        /// You can call passed delegate from js.
        /// All its exceptions will be passed to js.
        /// If delegate return Task - it will be converted to promise.
        /// </summary>
        /// <typeparam name="TArguments">Deserialize json params to current type.</typeparam>
        /// <returns></returns>
        public static void BindToJs<TArguments, TResult>(
            this IImprovedWebView @this, 
            Func<TArguments, TResult> func, 
            string functionName,
            string jsObjectName = "Native"
        )
        {
            @this.BindToJs(func.Method, func, functionName, jsObjectName);
        }

        /// <summary>
        /// Add all public methods of current object to js.
        /// Add methods signature too.
        /// Support all <see cref="BindToJs{TArguments, TResult}(IImprovedWebView, Func{TArguments, TResult}, string, string)"/>
        /// features.
        /// </summary>
        public static void BindToJs(
            this IImprovedWebView @this, 
            object proxyObject, 
            string jsObjectName
        )
        {
            if (proxyObject == null) throw new ArgumentNullException(nameof(proxyObject));
            var methods=proxyObject
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var mi in methods)
            {
                @this.BindToJs(mi, jsObjectName, mi.Name, jsObjectName);
            }
        }
    }
}