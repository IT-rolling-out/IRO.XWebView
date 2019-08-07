﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IRO.ImprovedWebView.Core.Models
{
    public class LoadResult
    {
        public string Url { get; set; }

        public LoadResult() { }

        public LoadResult(string url)
        {
            Url = url;
        }
    }
}
