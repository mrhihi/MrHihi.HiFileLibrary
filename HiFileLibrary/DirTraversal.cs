using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MrHihi.HiFileLibrary
{
    public class DirTraversal
    {
        #region 事件
        public class TraversalingEventArg : EventArgs
        {
            public string File { get; set; } = null;
        }

        public delegate void TraversalingEventHandler(object sender, TraversalingEventArg e);

        public event TraversalingEventHandler Traversaling;

        public class TraversalingDirEventArg : EventArgs
        {
            public string Dir { get; set; } = null;
        }

        protected virtual void OnTraversaling(TraversalingEventArg e)
        {
            TraversalingEventHandler handler = Traversaling;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public delegate void TraversalingDirEventHandler(object sender, TraversalingDirEventArg e);

        public event TraversalingDirEventHandler TraversalingDir;

        protected virtual void OnTraversalingDir(TraversalingDirEventArg e)
        {
            TraversalingDirEventHandler handler = TraversalingDir;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        public DirTraversal()
        {
        }

        public string ExcludeDirs { get; set; } = "";
        public string IncludeFileNames { get; set; } = "";

        protected string[] GetFiles(string dir)
        {
            string[] result;
            try
            {
                if (this.IncludeFileNames == "")
                {
                    result = Directory.GetFiles(dir);
                }
                else
                {
                    result = this.IncludeFileNames.Split(",;|".ToCharArray())
                                    .SelectMany(filter =>
                                                    Directory.GetFiles(dir, filter, SearchOption.TopDirectoryOnly)).ToArray();
                }
            }
            catch (Exception) { result = new string[] { }; }
            return result;
        }

        private const int MAX_LEVEL = int.MaxValue;
        private int _maxLevel = -1;

        public void FolderRun(string dir, int maxLevel = MAX_LEVEL)
        {
            FolderRun(new string[] { dir }, maxLevel);
        }

        /// <summary>
        /// 走訪指定的目錄，指定遞迴層數
        /// </summary>
        /// <param name="dirs"></param>
        /// <param name="maxLevel"></param>
        public void FolderRun(string[] dirs, int maxLevel = MAX_LEVEL)
        {
            _maxLevel = maxLevel;
            _folderRun(dirs, 0);
        }

        /// <summary>
        /// 走訪指定的目錄，指定遞迴層數
        /// </summary>
        /// <param name="dirs"></param>
        /// <param name="level"></param>
        private void _folderRun(string[] dirs, int level)
        {
            if (level > _maxLevel) return;

            Parallel.ForEach(dirs, dir =>
            {
                if (!string.IsNullOrEmpty(ExcludeDirs) && ExcludeDirs.Split(',', ';', '|').Any(x => dir.IndexOf(x, StringComparison.CurrentCultureIgnoreCase)>=0)) return;
                try
                {
                    _folderRun(Directory.GetDirectories(dir), level + 1);
                }
                catch (Exception) { };
                OnTraversalingDir(new HiFileLibrary.DirTraversal.TraversalingDirEventArg() { Dir = dir });
                _fileRun(GetFiles(dir));
            });
        }

        private void _fileRun(string[] files)
        {
            Parallel.ForEach(files, file => {
                OnTraversaling(new TraversalingEventArg() { File = file });
            });
        }
    }
}
