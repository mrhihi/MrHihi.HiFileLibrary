using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MrHihi.HiFileLibrary.FileOperation.Model;

namespace MrHihi.HiFileLibrary.FileOperation
{
    public class Replactor
    {
        IList<MatchData> matchData;
        string searchText;
        string replacement;

        public Replactor(IList<MatchData> matchData, string replacement)
        {
            this.matchData = matchData;
            this.searchText = matchData.First().SearchText;
            this.replacement = replacement;
        }

        public string Replace(long lineNumber, string lineData)
        {
            string result = lineData;

            var matchlines = matchData.Where(x => x.MatchLocation.LineNumber == lineNumber);
            result = _do_replace(matchlines, result);

            return result;
        }

        private string _do_replace(IEnumerable<MatchData> matchlines, string source)
        {
            string result = source;
            int matchCount = matchlines.Count();
            if (matchCount <= 0) return result;

            List<string> segement = new List<string>();
            int len = searchText.Length;
            int bpos = 0;
            int epos = 0;
            foreach (var m in matchlines.OrderBy(x => x.MatchLocation.Position))
            {
                epos = (int)m.MatchLocation.Position;
                segement.Add(result.Substring(bpos, epos - bpos) + replacement);
                bpos = epos + len;
            }
            int slen = source.Length;
            if (bpos < slen)
            {
                segement.Add(result.Substring(bpos, slen - bpos));
            }
            result = string.Join("", segement);

            return result;
        }

    }

}
