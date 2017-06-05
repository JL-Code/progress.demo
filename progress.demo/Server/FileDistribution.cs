using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace progress.demo
{
    /// <summary>
    /// 文件分发
    /// </summary>
    public class FileDistribution : IDisposable
    {
        #region 字段
        //文件流
        private FileStream fs;
        //缓冲池大小
        private int _bufferSize = 1024;
        //是否范围下载
        private bool _isRange = false;
        //数据开始位置
        private long _from;
        //数据结束位置
        private long _to;
        //文件长度（以字节为单位）
        private long _totalLength;
        #endregion

        #region 属性
        /// <summary>
        /// 缓冲池大小
        /// </summary>
        public int BufferSize
        {
            get
            {
                if (_bufferSize > _totalLength)
                {
                    _bufferSize = (int)_totalLength;
                }
                return _bufferSize;
            }
            set
            {
                if (value > 0)
                {
                    _bufferSize = value;
                }
            }
        }
        /// <summary>
        /// 文件读取超时时间 默认1分钟超时
        /// </summary>
        public int ReadTimeout { get; set; } = 60000;
        /// <summary>
        /// 上次修改文件的日期
        /// </summary>
        public DateTimeOffset? ModificationDate { get; set; }
        /// <summary>
        /// 创建文件的日期
        /// </summary>
        public DateTimeOffset? CreationDate { get; set; }
        /// <summary>
        /// 上次读取文件的日期
        /// </summary>
        public DateTimeOffset? ReadDate { get; set; }
        /// <summary>
        /// 是否分部请求
        /// </summary>
        public bool IsRange { get => _isRange; }

        #endregion

        #region 公共方法
        public FileDistribution(string path)
        {
            if (!IsExists(path))
                throw new FileNotFoundException("该资源不存在");
            var fi = new FileInfo(path);
            ModificationDate = fi.LastWriteTime;
            CreationDate = fi.CreationTime;
            ReadDate = fi.LastAccessTime;
            fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            _totalLength = fs.Length;
        }

        /// <summary>
        /// 分发文件
        /// </summary>
        /// <param name="hrm">请求信息</param>
        /// <returns></returns>
        public DistributionValue Distribution(HttpRequestMessage hrm)
        {
            AnalyzeRequestRanges(hrm);
            if (_isRange)
                return Distribution(_from, _to);
            else
                return Distribution();
        }

        /// <summary>
        /// 根据请求范围分发文件
        /// </summary>
        /// <param name="from">开始发送数据的位置。</param>
        /// <param name="to">结束发送数据的位置。</param>
        /// <returns>返回from到to的文件字节</returns>
        public DistributionValue Distribution(long from, long to)
        {

            if (!fs.CanRead)
                throw new IOException("当前流不可读！");
            TryDistribution(from, to);
            var buffer = new byte[_bufferSize];
            //TODO Read的offset偏移量针对流还是byte数组？？
            // 答案：偏移量是指‘接收缓冲区’，既Buffer的偏移量，而不是数据流的偏移量。
            fs.Position = from;
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            return new DistributionValue()
            {
                Content = buffer,
                From = from,
                To = to,
                IsRange = IsRange,
                TotalLength = _totalLength,
                ModificationDate = ModificationDate,
                CreationDate = CreationDate,
                ReadDate = ReadDate
            };
        }

        /// <summary>
        /// 分发文件
        /// </summary>
        /// <returns></returns>
        public DistributionValue Distribution()
        {
            var buffer = new byte[_bufferSize];
            if (!fs.CanRead)
                throw new IOException("当前流不可读！");
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            var result = new DistributionValue()
            {
                Content = buffer,
                From = 0,
                To = _totalLength,
                IsRange = IsRange,
                TotalLength = _totalLength,
                ModificationDate = ModificationDate,
                CreationDate = CreationDate,
                ReadDate = ReadDate
            };
            return result;
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="path">文件完整路径</param>
        /// <returns></returns>
        public bool IsExists(string path)
        {
            return File.Exists(path);
        }
        public void Dispose()
        {
            fs?.Close();
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 解析文件下载范围
        /// </summary>
        private void AnalyzeRequestRanges(HttpRequestMessage hrm)
        {
            var range = hrm.Headers.Range;
            if (range != null)
            {
                _isRange = true;
                var ranges = range.Ranges.ToList();
                if (ranges.Count == 1)
                {//单范围
                    _from = ranges[0].From.GetValueOrDefault();
                    _to = ranges[0].To.GetValueOrDefault();
                    BufferSize = (int)(_to - _from) + 1;
                }
                else
                { //TODO 多范围请求暂未实现
                    ranges.ForEach(kv =>
                    {

                    });
                }
            }
            else
            {
                BufferSize = (int)_totalLength;
            }
        }

        /// <summary>
        /// 尝试分发指定范围的文件
        /// </summary>
        /// <param name="from">开始发送数据的位置</param>
        /// <param name="to">结束发送数据的位置</param>
        /// <returns>指定范围文件</returns>
        private void TryDistribution(long from, long to)
        {

            if (from < 0)
            {
                from = 0;
            }
            if (from >= _totalLength)
            {
                from = _totalLength - 1;
            }
            if (to >= _totalLength)
            {
                to = _totalLength - 1;
            }
        }
        #endregion

    }
}