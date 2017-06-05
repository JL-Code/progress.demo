using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace progress.demo
{
    public class ProgressManager
    {
        /// <summary>
        /// 将任务进度分为100份
        /// </summary>
        private readonly int _progress = 100;
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted => _progress == Progress;
        /// <summary>
        /// 任务进度
        /// </summary>
        public int Progress { get; set; }
        /// <summary>
        /// 设置任务进度
        /// </summary>
        /// <param name="func"></param>
        public void SetProgress(Func<int> func)
        {
            while (!IsCompleted)
            {
                Progress = func.Invoke();
            }
        }
    }
}