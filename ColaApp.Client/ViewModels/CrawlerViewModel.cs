using ColaApp.Client.ViewModels.Base;
using ColaApp.Core;
using GalaSoft.MvvmLight.Command;
using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ColaApp.Client.ViewModels
{
    class CrawlerViewModel : ViewModelBaseEx
    {
        const string websiteUrl = "https://www.biquyun.com";
        string bookUrl = websiteUrl + "/1_1872/";
        List<string> chapterUrls;
        private List<Task> tasks = new List<Task>();

        public ICommand DownloadCommand { get; set; }
        int totalTaskCount = 2;

        public CrawlerViewModel()
        {
            this.DownloadCommand = new RelayCommand(() => DownloadNovel());
        }
        string tempPath;
        private async void DownloadNovel()
        {
            HtmlWeb webClient = new HtmlWeb();       
            HtmlDocument doc = await webClient.LoadFromWebAsync(bookUrl, Encoding.GetEncoding("gbk"));
            string novelName = doc.DocumentNode.SelectSingleNode("//h1").InnerText;
            var node = doc.GetElementbyId("list");
            chapterUrls = new List<string>(node.SelectNodes(".//a[@href]").Select(o=>o.Attributes["href"].Value));

            string novelPath = Directory.GetCurrentDirectory() + @"\Novel\" + novelName + ".txt";  //小说地址
            tempPath = Directory.GetCurrentDirectory() + @"\Novel\Temp\" + novelName + Path.DirectorySeparatorChar ;  //小说地址
            File.Create(novelPath).Close();
            Directory.CreateDirectory(tempPath);
            

            for (int i = 0; i < chapterUrls.Count; i++)
            {
                if (tasks.Any())
                {
                    Task.WaitAny(tasks.ToArray());
                    var completedTasks = tasks.Where(o => o.IsCompleted).ToList();
                    for (int j = 0; j < completedTasks.Count(); j++)
                    {                        
                        tasks.Remove(completedTasks[j]);
                        tasks.Add(Task.Factory.StartNew(()=>DownloadChapter(i)));
                        completedTasks[j].Dispose();                
                        continue;
                    }
                }
                else
                {
                    for (int j = 0; j < totalTaskCount; j++)
                    {
                        tasks.Add(Task.Factory.StartNew(() => DownloadChapter(i)));
                    }
                }
            }
         
        }

        private async void DownloadChapter(int index)
        {
            HtmlWeb webClient = new HtmlWeb();
            HtmlDocument doc = await webClient.LoadFromWebAsync(websiteUrl + chapterUrls[index], Encoding.GetEncoding("gbk"));
            //获取正文内容
            string content = HtmlEntity.DeEntitize(doc.GetElementbyId("content").InnerText);

            string tmpFileName = tempPath + (index + 1).ToString() + ".txt";
            File.Delete(tmpFileName);
            File.Create(tmpFileName).Close();
            using (StreamWriter tempChapterWriter = new StreamWriter(tmpFileName))
            {
                await tempChapterWriter.WriteLineAsync(content);
            }
            
        }
    }
}
