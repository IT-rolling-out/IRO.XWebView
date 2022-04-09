using System.Collections.Generic;
using System.IO;

namespace PuppeteerSharp.Extensions.ChromeFinder
{
    public class ChromeFinder
    {
        public IList<string> Find()
        {

            var suffixes = new string[]
            {
                "\\Google\\Chrome SxS\\Application\\chrome.exe",
                "\\Google\\Chrome\\Application\\chrome.exe",
                "\\chrome-win32\\chrome.exe",
                "\\Chromium\\Application\\chrome.exe",
                "\\Google\\Chrome Beta\\Application\\chrome.exe"
            };

            var prefixes = new string[]
            {
                "C:\\Program Files",
                "D:\\Program Files",
                "C:\\Program Files (x86)",
                "D:\\Program Files (x86)"
            };

            var accessibleFiles = new List<string>();
            foreach (var pref in prefixes)
            {
                foreach (var suf in suffixes)
                {
                    var filePath = pref + suf;
                    if (CanAccess(filePath))
                    {
                        accessibleFiles.Add(filePath);
                    }
                }
            }
            return accessibleFiles;
        }

        private bool CanAccess(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            if (IsFileLocked(filePath))
            {
                return false;
            }
            return true;
        }

        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }

    }
}
