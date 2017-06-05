using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using progress.demo;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Text;

namespace progress.tests
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// 显示任务进度的委托
        /// </summary>
        /// <param name="totalSize">资源总量</param>
        /// <param name="currentSize">当前资源大小</param>
        public delegate void ShowProgressDelegate(int totalStep, int currentStep);
        DateTime StartTime;
        public IHubConnectionContext<dynamic> Clients { get; set; }
        [TestInitialize]
        public void Initialize()
        {
            Clients = GlobalHost.ConnectionManager.GetHubContext<ProgressTipHub>().Clients;
        }
        [TestMethod]
        public void DownloadTest()
        {
            Download();
        }
        public async void Download()
        {
            var downUri = "http://123.147.165.14:9999/sw.bos.baidu.com/sw-search-sp/software/5d7946e712c83/npp_7.4.1_Installer.exe";
            var destinationFilePath = @"C:\Users\jiang\Documents\Visual Studio 2017\Projects\progress.demo\npp_7.4.1_Installer.exe";
            try
            {
                using (var client = new HttpClientDownloadWithProgress(downUri, destinationFilePath))
                {
                    client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                    {
                        Console.WriteLine($"{progressPercentage}% ({totalBytesDownloaded}/{totalFileSize})");
                    };
                    await client.StartDownload();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void PartialDownload_WebAPI_NotNull()
        {
            var downUri = "http://123.147.165.14:9999/sw.bos.baidu.com/sw-search-sp/software/5d7946e712c83/npp_7.4.1_Installer.exe";
            downUri = @"http://localhost:62208/api/values/file";
            var name = @"bootstrap.css";
            var destinationFilePath = @"C:\Users\jiang\Documents\Visual Studio 2017\Projects\progress.demo";
            StartTime = DateTime.Now;
            var downloader = new Downloader(downUri, destinationFilePath, name);
            try
            {
                while (!downloader.IsCompleted)
                {
                    downloader.Download();
                    new ShowProgressDelegate(ShowProgressBySignalR).Invoke(100, (int)downloader.CurrentProgress);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ShowProgress(int totalStep, int currentStep)
        {
            var diff = (DateTime.Now - StartTime);
            Console.WriteLine($"进度:{currentStep}%");
            var timeStr = $"当前已耗时：{diff.Hours}:{diff.Minutes}:{diff.Seconds}:{diff.Milliseconds}";
            Console.WriteLine(timeStr);
        }

        public void ShowProgressBySignalR(int totalStep, int currentStep)
        {
            var diff = (DateTime.Now - StartTime);
            var timeStr = $"当前已耗时：{diff.Hours}:{diff.Minutes}:{diff.Seconds}:{diff.Milliseconds}";
            Clients.All.updateProgress(currentStep, timeStr);
        }

        [TestMethod]
        public void RoadFile_TXT_IsReadable()
        {
            var buffer = new byte[1024];
            var txt = string.Empty;
            var path = Path.Combine(@"C:\Users\jiang\Documents\visual studio 2017\Projects\progress.demo\progress.demo", "Content", "1.txt");
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                fs.Read(buffer, 0, buffer.Length);
                txt = Encoding.UTF8.GetString(buffer);
            }
            Console.WriteLine(txt);
        }

        [TestMethod]
        public void FileDistRoadFile_TXT_IsReadable()
        {
            var path = Path.Combine(@"C:\Users\jiang\Documents\visual studio 2017\Projects\progress.demo\progress.demo", "Content", "1.txt");
            var fd = new FileDistribution(path);
            var result = fd.Distribution();
            var txt = Encoding.UTF8.GetString(result.Content);
            Console.WriteLine(txt);
        }
    }
}
