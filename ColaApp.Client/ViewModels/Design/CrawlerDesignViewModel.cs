using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColaApp.Client.ViewModels.Design
{
    class CrawlerDesignViewModel : CrawlerViewModel
    {
        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static CrawlerDesignViewModel Instance => new CrawlerDesignViewModel();

        public CrawlerDesignViewModel()
        {
            this.Chapters = new System.Collections.ObjectModel.ObservableCollection<DataModels.ChapterDataModel>()
                {
                    new DataModels.ChapterDataModel{Title="第一章 林动 Design "},
                    new DataModels.ChapterDataModel{Title="第二章 通背拳 Design "},
                    new DataModels.ChapterDataModel{Title="第三章 古怪的石池 Design "},
                    new DataModels.ChapterDataModel{Title="第四章 石池之秘 Design "},
                    new DataModels.ChapterDataModel{Title="第五章 神秘石符 Design "},

                }
            ;
        }
    }
}
