using System;
using System.Collections.Generic;

namespace MrHihi.HiFileLibrary.FileOperation.Search
{
    public class SearchByText: ISearcher
    {
        public bool Test(string source, string pattern, bool ignoreCase)
        {
            int[] empty;
            return Test(source, pattern, ignoreCase, out empty);
        }

        public bool Test(string source, string pattern, bool ignoreCase, out int[] position)
        {
            int sourceLen = source.Length;
            int patternLen = pattern.Length;
            var outputResult = new List<int>();

            if (source == "" || patternLen == 0)
            {
                position = outputResult.ToArray();
                return false;
            }

            // 同一行裡出現多次通通紀錄下來
            int pos = 0;
            while(pos >-1)
            {
                if (ignoreCase)
                {
                    pos = source.IndexOf(pattern, pos, StringComparison.CurrentCultureIgnoreCase);
                }
                else
                {
                    pos = source.IndexOf(pattern, pos, StringComparison.CurrentCulture);
                }
                if (pos >-1)
                {
                    outputResult.Add(pos);
                    pos += patternLen;
                }
            }

            position = outputResult.ToArray();
            return outputResult.Count > 0;

        }
    }
}