using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MrHihi.HiFileLibrary.FileOperation.Search
{
    public class SearchByRegex: ISearcher
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
            var option = RegexOptions.Compiled | (ignoreCase?RegexOptions.IgnoreCase:0);
            Regex re = new Regex(pattern, option);
            var mh = re.Match(source);
            while(mh.Success) {
                outputResult.Add(mh.Index);
                mh = mh.NextMatch();
            }
            position = outputResult.ToArray();
            return outputResult.Count > 0;
        }
    }
}