using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColaApp.Client.DataModels
{
    class ChapterDataModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string SavedPath { get; set; }
        public bool IsDownloadOK => !string.IsNullOrEmpty(SavedPath);
        public string Content { get; internal set; }
    }
}
