using System;

namespace progress.demo
{
    /// <summary>
    /// 文件分发返回值
    /// </summary>
    public class DistributionValue
    {
        /// <summary>
        /// 分部资源
        /// </summary>
        public byte[] Content { get; set; }
        /// <summary>
        /// 资源开始位置
        /// </summary>
        public long From { get; set; }
        /// <summary>
        /// 资源结束位置
        /// </summary>
        public long To { get; set; }
        /// <summary>
        /// 服务器资源总长度
        /// </summary>
        public long TotalLength { get; set; }

        public bool IsRange { get; set; }

        public DateTimeOffset? ModificationDate { get; set; }
        public DateTimeOffset? CreationDate { get; set; }
        public DateTimeOffset? ReadDate { get; set; }
    }
}