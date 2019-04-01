using ColaApp.Client.DataModels;
using ColaApp.Client.ViewModels.Base;
using ColaApp.Core;
using Dna;
using GalaSoft.MvvmLight.Command;
using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Dna.FrameworkDI;
using static ColaApp.Core.DI.CoreDI;
using System.Threading;
using MahApps.Metro.Controls.Dialogs;

namespace ColaApp.Client.ViewModels
{
    class CrawlerViewModel : ViewModelBaseEx
    {
        const string websiteUrl = "https://www.biquyun.com";
        string bookUrl = websiteUrl + "/1_1408/";

        #region Public Properties
        /// <summary>
        /// 章节列表
        /// </summary>
        public ObservableCollection<ChapterDataModel> Chapters { get; protected set; }
        public ICommand DownloadCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand TestCommand { get; set; }
        public double Progress => 100 * ((this.Chapters?.Count).GetValueOrDefault() == 0 ? 0 : (double)DownloadedCount / this.Chapters.Count);
        public string Status { get; private set; }

        public string ErrorInfo => sbErrorInfo.ToString();
        /// <summary>
        /// 当前错误信息
        /// </summary>
        public string CurrentError {
            get => currentError;
            private set {
                lock(locker)
                {
                    currentError = value;
                    this.sbErrorInfo.AppendLine(value);
                    RaisePropertyChanged(nameof(ErrorInfo)); //需要通知ErrorInfo更改
                }
            } }

        /// <summary>
        /// 已下载数量
        /// </summary>
        public int DownloadedCount { get; private set; }
        #endregion

        /// <summary>
        /// 总线程数
        /// </summary>
        private int totalTaskCount = 4;
        private List<Task> tasks = new List<Task>();
        private HtmlWeb webClient = new HtmlWeb();
        private object locker = new object();
        private StringBuilder sbErrorInfo = new StringBuilder();
        private string currentError;
        /// <summary>
        /// 仍需下载的章节Index队列
        /// </summary>
        private BlockingCollection<int> downloadingIndexs = new BlockingCollection<int>();

        public CrawlerViewModel()
        {
            this.DownloadCommand = new RelayCommand(async () => await DownloadNovel());
            this.CancelCommand = new RelayCommand(() => this.cts.Cancel());
            this.TestCommand = new RelayCommand(async () => { await DialogCoordinator.Instance.ShowMessageAsync(this, "test title", "test message", MessageDialogStyle.Affirmative); });
        }

        string tempPath;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private async Task DownloadNovel()
        {
            try
            {
                string novelName = null;
                try
                {
                    SetStatus("获取章节列表开始");
                    HtmlDocument doc = await webClient.LoadFromWebAsync(bookUrl, Encoding.GetEncoding("gbk"), cts.Token);
                    novelName = doc.DocumentNode.SelectSingleNode("//h1").InnerText;
                    var node = doc.GetElementbyId("list");
                    Chapters = new ObservableCollection<ChapterDataModel>(node.SelectNodes(".//a[@href]").Select(
                        o => new ChapterDataModel() { Url = websiteUrl + o.Attributes["href"].Value , Title = o.InnerText})
                    );
                    //添加需要下载的索引
                    for (int i = 0; i < Chapters.Count; i++)
                    {
                        downloadingIndexs.Add(i);
                    }
                    // Need to do this to keep foreach below from hanging
                    downloadingIndexs.CompleteAdding();
                    SetStatus("获取章节列表完成");
                }
                catch (Exception e)
                {
                    SetStatus($"获取章节列表错误");
                    Logger.LogErrorSource(CurrentError, exception: e);
                    return;
                }

                #region 创建小说临时对应路径
                tempPath = Directory.GetCurrentDirectory() + @"\Novel\Temp\" + novelName + Path.DirectorySeparatorChar;  //小说临时地址
                Directory.CreateDirectory(tempPath);
                #endregion

                SetStatus("下载小说所有章节开始");
                Task[] consumers = new Task[totalTaskCount];

                for (int i = 0; i < totalTaskCount; ++i)
                {
                    int consumerId = i;
                    Task task = Task.Run(() => DownloadChapterAsync(consumerId, cts));
                    consumers[i] = task;
                }

                await Task.WhenAll(consumers);
                SetStatus("下载小说所有章节结束");

                #region 合并章节
                string novelPath = Directory.GetCurrentDirectory() + @"\Novel\" + novelName + ".txt";  //小说本地存储路径
                File.Create(novelPath).Close();
                await MergerChapterAsync(novelPath);
                SetStatus("合并所有所有章节完毕");
                #endregion
            }
            catch (OperationCanceledException)
            {
                SetStatus($"用户取消下载");
                return;
            }
        }

        /// <summary>
        /// 合并章节
        /// </summary>
        /// <returns></returns>
        private Task MergerChapterAsync(string novelPath)
        {
            return Task.Run(()=>
            {
                var inFiles = this.Chapters.Where(o => o.IsDownloadOK).Select(o => o.SavedPath).ToArray();
                FileManager.CombineFile(inFiles, novelPath);
            });
        }

        /// <summary>
        /// 下载小说章节
        /// </summary>
        /// <param name="index">索引号</param>
        /// <returns></returns>
        private async Task DownloadChapterAsync(int consumerID, CancellationTokenSource cts)
        {

            Console.WriteLine($"Consumer {consumerID} is starting.");

            foreach (var chapterIndex in this.downloadingIndexs.GetConsumingEnumerable())
            {
                ChapterDataModel currentChapter = null;
                try
                {
                    HtmlDocument doc = await webClient.LoadFromWebAsync(Chapters[chapterIndex].Url, Encoding.GetEncoding("gbk"), cts.Token);
                    currentChapter = Chapters[chapterIndex];
                    //获取正文内容
                    currentChapter.Content = HtmlEntity.DeEntitize(doc.GetElementbyId("content").InnerText);

                    string tmpFilePath = tempPath + (chapterIndex + 1).ToString() + ".txt";
                    File.Delete(tmpFilePath);
                    File.Create(tmpFilePath).Close();
                    using (StreamWriter tempChapterWriter = new StreamWriter(tmpFilePath))
                    {
                        tempChapterWriter.WriteLine(currentChapter.Title);
                        tempChapterWriter.WriteLine(currentChapter.Content);
                        currentChapter.SavedPath = tmpFilePath;
                        lock (locker)
                        {
                            this.DownloadedCount++;
                        }
                    }
                    CurrentError = $"下载{currentChapter.Title}成功";
                }
                catch (OperationCanceledException)
                {
                    throw; //在此抛出，让调用方法进行捕获
                }
                catch (Exception e)
                {
                    CurrentError = $"下载章节错误，索引号:{chapterIndex}，{e.Message}";
                    Logger.LogErrorSource(CurrentError, exception: e);
                }
                
            }

            Console.WriteLine($"Consumer {consumerID} is finished.");
        }

        #region Private Helpers

        private void SetStatus(string newStatus) => this.Status = newStatus;

        #endregion
    }
}
