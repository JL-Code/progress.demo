using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace progress.demo
{
    /// <summary>
    /// 文件下载工具类 带进度提示
    /// </summary>
    public class Downloader : IDisposable
    {

        #region 字段
        //文件下载Uri
        private readonly string _requestUri;
        //文件目录
        private readonly string _dirctory;
        //文件名称
        private string _name;
        //HTTP模拟客户端
        private HttpClient _client;
        //超时时间
        private TimeSpan _timeout = TimeSpan.FromMilliseconds(30000);
        //当前已下载文件大小
        private long _currentSize = 0;
        //文件总大小
        private long _totalSize;
        //分片
        private long _slice = 102400;
        //文件流
        private FileStream _fs;
        #endregion

        #region 属性
        /// <summary>
        /// 分片大小
        /// </summary>
        public long Slice { get => _slice; set => _slice = value; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted { get; private set; }
        /// <summary>
        /// 当前进度的百分数
        /// </summary>
        public float CurrentProgress
        {
            get
            {
                if (_totalSize != 0)
                {
                    Console.WriteLine($"CurrentProgress==>  _currentSize={_currentSize} _totalSize={_totalSize}");
                    return (float)_currentSize * 100 / _totalSize;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout
        {
            get => _timeout;
            set => _timeout = value;
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get => _name; }

        #endregion

        #region 公共方法
        /// <summary>
        /// 构造一个下载器 指定文件Uri 文件存放文件目录 文件采用默认名称
        /// </summary>
        /// <param name="requestUri">文件Uri</param>
        /// <param name="dirctory">文件目录</param>
        public Downloader(string requestUri, string dirctory)
        {
            _requestUri = requestUri;
            _dirctory = dirctory;
            Init();
        }
        /// <summary>
        /// 构造一个下载器 指定文件Uri 文件存放文件目录及文件名称
        /// </summary>
        /// <param name="requestUri">文件Uri</param>
        /// <param name="dirctory">文件目录</param>
        /// <param name="name">文件名称</param>
        public Downloader(string requestUri, string dirctory, string name)
        {
            _requestUri = requestUri;
            _dirctory = dirctory;
            _name = name;
            Init();
        }
        /// <summary>
        /// 下载
        /// </summary>
        public void Download()
        {
            //请求范围计算  从0开始 为保证整数段 减1 
            long from = _currentSize;
            long to = from + _slice - 1;
            if (_totalSize == 0)
                GetTotalSize();
            if (to >= _totalSize && _totalSize > 0)
            {
                to = _totalSize - 1;
            }
            Download(from, to);
        }
        /// <summary>
        /// 分段下载
        /// </summary>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        public void Download(long from, long to)
        {
            if (_totalSize == 0)
                GetTotalSize();
            Console.WriteLine($"CurrentProgress==>  _currentSize={_currentSize} _totalSize={_totalSize}");
            if (_currentSize >= _totalSize)
            {
                IsCompleted = true;
                return;
            }
            var request = new HttpRequestMessage(HttpMethod.Get, _requestUri);
            request.Headers.Range = new RangeHeaderValue(from, to);
            var response = _client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            using (var bytesRep = response.Content.ReadAsByteArrayAsync())
            {
                var fileByteArr = bytesRep.Result;
                _currentSize += fileByteArr.Length;
                _fs.WriteAsync(fileByteArr, 0, fileByteArr.Length);
            };
            //不支持断点下载
            if (response.Content.Headers.ContentRange == null)
            {
                IsCompleted = true;
            }
        }

        /// <summary>
        /// 释放非托管文件
        /// </summary>
        public void Dispose()
        {
            _fs?.Close();
            _client?.Dispose();
        }
        #endregion

        #region 私有方法
        private string GetFileNameFromUri(string uri)
        {
            return "";
        }
        //初始文件存放路径
        private void Init()
        {
            if (!Directory.Exists(_dirctory))
                Directory.CreateDirectory(_dirctory);
            if (string.IsNullOrEmpty(_name))
                _name = _requestUri.Substring(_requestUri.LastIndexOf('/') + 1);

            var fullPath = Path.Combine(_dirctory, _name);
            _fs = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _client = new HttpClient() { Timeout = _timeout };
        }
        /// <summary>
        /// 获取文件总大小
        /// </summary>
        /// <returns></returns>
        private void GetTotalSize()
        {
            var response = _client.GetAsync(_requestUri, HttpCompletionOption.ResponseHeadersRead).Result;
            _totalSize = response.Content.Headers.ContentLength.GetValueOrDefault();
        }
        #endregion

    }
}