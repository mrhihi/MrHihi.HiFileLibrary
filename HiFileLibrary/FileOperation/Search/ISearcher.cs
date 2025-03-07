using System;
using System.Collections.Generic;

namespace MrHihi.HiFileLibrary.FileOperation.Search
{
    public interface ISearcher
    {
        bool Test(string source, string pattern, bool ignoreCase);
        bool Test(string source, string pattern, bool ignoreCase, out int[] position);
    }
}