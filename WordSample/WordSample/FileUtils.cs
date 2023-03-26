using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WordSample
{
    internal class FileUtils
    {
        public static List<string> MergeTextContents(string[] tempFiles)
        {
            var contents = new List<string>();
            foreach (var tempFile in tempFiles)
            {
                if (File.Exists(tempFile))
                {
                    contents.AddRange(File.ReadAllLines(tempFile, Encoding.Default));
                }
            }
            return contents;
        }

        public static void DeleteFiles(string[] tempFiles)
        {
            foreach (var tempFile in tempFiles)
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}
