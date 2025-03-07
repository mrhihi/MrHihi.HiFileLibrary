using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MrHihi.HiFileLibrary.FileOperation.Model;
using MrHihi.HiFileLibrary.FileOperation.Search;

namespace MrHihi.HiFileLibrary.FileOperation
{
    public class FileSearcher
    {

        #region 事件
        public class SearchingEventArg : EventArgs
        {
            public string File { get; set; } = "";
        }

        public delegate void SearchingEventHandler(object sender, SearchingEventArg e);

        public event SearchingEventHandler Searching;

        protected virtual void OnSearching(SearchingEventArg e)
        {
            SearchingEventHandler handler = Searching;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        public string SearchTarget { get; set; } = "";
        public string SearchInclude { get; set; } = "";
        public bool IgnoreCase { get; set; } = true;
        protected MD5 md5 { get; } = MD5.Create();

        public IList<MatchData> Search(string filePath)
        {
            ISearcher s = new SearchByText();
            return Search(filePath, s);
        }

        public IList<MatchData> Search(string filePath, ISearcher searcher)
        {
            OnSearching(new SearchingEventArg() { File = filePath });

            var result = new List<MatchData>();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var fsr = new StreamReader(fs, Encoding.GetEncoding(950), true))
                {
                    long linenumber = 0;
                    var encoding = fsr.CurrentEncoding;
                    while (!fsr.EndOfStream)
                    {
                        linenumber++;
                        string l = fsr.ReadLine();
                        int[] matchesPos;
                        if (!searcher.Test(l, SearchTarget, this.IgnoreCase, out matchesPos)) continue; // 找不到 SearchTarget 就換下一行
                        if (SearchInclude != "" && !searcher.Test(l, SearchInclude, this.IgnoreCase)) continue; // 有指定 SearchInclude 的話，找到的行必須包含 SearchInclude 的內容

                        foreach (int pos in matchesPos)
                        {
                            var md = new MatchData()
                            {
                                MatchFile = filePath,
                                SearchText = SearchTarget,
                                MatchLineText = l,
                                MatchLocation = new Location() { LineNumber = linenumber, Position = pos }
                            };
                            result.Add(md);
                        }
                    }

                    if (result.Count > 0)
                    {
                        fs.Position = 0;
                        string checksum = BitConverter.ToString(md5.ComputeHash(fs)); // 紀錄找到的檔案 MD5 ，之後要置換利用 MD5 檢查檔案是否有被異動過
                        result.ForEach((r) => { r.FileChecksum = checksum; });
                    }
                }
            }
            return result;
        }
    }
}
