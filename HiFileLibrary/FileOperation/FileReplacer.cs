using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using MrHihi.HiFileLibrary.FileOperation.Model;

namespace MrHihi.HiFileLibrary.FileOperation
{
    public class FileReplacer
    {
        protected MD5 md5 { get; } = MD5.Create();
        IList<MatchData> matches;
        public FileReplacer(IList<MatchData> matches)
        {
            this.matches = matches;
        }

        private void replace(IGrouping<string, MatchData> matchData, string replacement)
        {
            var rtor = new Replactor(matchData.ToList(), replacement);
            using (var fs = new FileStream(matchData.Key, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                fs.Position = 0;
                string checksum = BitConverter.ToString(md5.ComputeHash(fs));
                if (checksum != matchData.First().FileChecksum) throw new Exception("Warning! The file has been modified!");
                fs.Position = 0;

                using (var fsr = new StreamReader(fs, Encoding.GetEncoding(950), true))
                {
                    fsr.Peek(); // 先讀一下，判斷編碼
                    long linenumber = 0;
                    var encoding = fsr.CurrentEncoding;
                    using (var mem = new MemoryStream())
                    {
                        using (var w2mem = new StreamWriter(mem, encoding, 1024, true))
                        {
                            while (!fsr.EndOfStream)
                            {
                                linenumber++;
                                string l = fsr.ReadLine();
                                l = rtor.Replace(linenumber, l);
                                w2mem.WriteLine(l);
                            }
                            w2mem.Flush();
                        }
                        fsr.Close();
                        mem.Seek(0, SeekOrigin.Begin);
                        using (var msr = new StreamReader(mem, encoding, true, 1024, true))
                        {
                            using (var w2file = new StreamWriter(matchData.Key, false, encoding))
                            {
                                w2file.Write(msr.ReadToEnd());
                            }
                        }
                    }
                }
            }
        }

        public void Replace(string replacement)
        {
            var fileGroup = matches.GroupBy(x => x.MatchFile);
            foreach(var matchesInFile in fileGroup)
            {
                replace(matchesInFile, replacement);
            }
        }
    }
}
