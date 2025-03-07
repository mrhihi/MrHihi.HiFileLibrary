using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrHihi.HiFileLibrary.FileOperation.Model
{
    public class MatchData
    {
        public string MatchFile { get; set; } = "";
        public string FileChecksum { get; set; } = "";
        public string SearchText { get; set; } = "";
        public string MatchLineText { get; set; } = "";
        public Location MatchLocation { get; set; } = null;

        public override string ToString()
        {
            return string.Format("{0} At {1}:{2} => {3}", 
                        SearchText, 
                        MatchLocation.LineNumber, 
                        MatchLocation.Position, 
                        MatchLineText);
        }
    }
}
