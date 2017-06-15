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

        [TestMethod]
        public void Compress()
        {
            CompressUtil.CompressFolder("E:\\ZapSoft", "E:\\03_ReleaseWebSite\\ZapSoft\\UpgradePackages\\ZapSoft.zip");
            var flag = File.Exists("E:\\03_ReleaseWebSite\\ZapSoft\\UpgradePackages\\ZapSoft.zip");
            Assert.AreEqual(flag, true);
        }

        [TestMethod]
        public void Decompress()
        {
            CompressUtil.DecompressFile("E:\\ZapSoft.zip", "E:\\");
            var flag = Directory.Exists("E:\\ZapSoft");
            Directory.Move("E:\\ZapSoft", "E:\\Test");
            Assert.AreEqual(flag, true);
        }
    }
}
