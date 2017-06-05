using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace progress.demo.Controllers
{
    public class ValuesController : ApiController
    {
        public delegate void ShowProgressDelegate(int totalStep, int currentStep);
        DateTime StartTime;
        public IHubConnectionContext<dynamic> Clients { get; set; }
        // Singleton instance
        //private readonly static Lazy<StockTicker> _instance = new Lazy<StockTicker>(
        //    () => new StockTicker(GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>().Clients));
        public ValuesController()
        {
            Clients = GlobalHost.ConnectionManager.GetHubContext<ProgressTipHub>().Clients;
        }
        public string Get()
        {
            return "测试";
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <returns></returns>
        [Route("api/values/resource")]
        [HttpGet]
        public IHttpActionResult Resource()
        {
            var browser = String.Empty;
            if (HttpContext.Current.Request.UserAgent != null)
            {
                browser = HttpContext.Current.Request.UserAgent.ToUpper();
            }
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "bootstrap.css");
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            FileStream fileStream = File.OpenRead(filePath);
            httpResponseMessage.Content = new StreamContent(fileStream);
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName =
                    browser.Contains("FIREFOX")
                        ? Path.GetFileName(filePath)
                        : HttpUtility.UrlEncode(Path.GetFileName(filePath))
            };

            return ResponseMessage(httpResponseMessage);
        }

        [Route("api/values/progress")]
        [HttpGet]
        public IHttpActionResult Progress(string name)
        {
            try
            {
                var downUri = @"http://123.147.165.14:9999/sw.bos.baidu.com/sw-search-sp/software/5d7946e712c83/npp_7.4.1_Installer.exe";
                if (!string.IsNullOrEmpty(name))
                {
                    downUri = @"http://localhost:62208/api/values/file?name=" + name;
                }
                var destinationFilePath = @"C:\Users\jiang\Documents\Visual Studio 2017\Projects\progress.demo";
                StartTime = DateTime.Now;
                var downloader = new Downloader(downUri, destinationFilePath, name);
                while (!downloader.IsCompleted)
                {
                    downloader.Download();
                    new ShowProgressDelegate(ShowProgressBySignalR).Invoke(100, (int)downloader.CurrentProgress);
                }
                downloader.Dispose();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            return Ok();
        }

        [Route("api/values/file")]
        [HttpGet]
        public IHttpActionResult DistributionFile(string name)
        {
            var req = Request;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", name);
            try
            {
                var fd = new FileDistribution(path);
                var dv = fd.Distribution(req);
                var response = new HttpResponseMessage()
                {
                    //设置响应实体首部字段
                    Content = new ByteArrayContent(dv.Content),
                    StatusCode = dv.IsRange ? HttpStatusCode.PartialContent : HttpStatusCode.OK
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentLength = dv.Content.LongLength;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = name,
                    ModificationDate = dv.ModificationDate,
                    ReadDate = dv.ReadDate,
                    CreationDate = dv.CreationDate,
                    Size = dv.TotalLength
                };
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(dv.From, dv.To, dv.TotalLength);
                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.ToString()),
                };
                return ResponseMessage(response);
            }
        }

        private void ShowProgressBySignalR(int totalStep, int currentStep)
        {
            var diff = (DateTime.Now - StartTime);
            var timeStr = $"当前已耗时：{diff.Hours}:{diff.Minutes}:{diff.Seconds}:{diff.Milliseconds}";
            Clients.All.updateProgress(currentStep, timeStr);
        }
    }
}
