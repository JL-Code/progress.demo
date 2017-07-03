using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace progress.demo
{
    public class SignalRManager
    {
        private HubConnection _hubConn;
        private IHubProxy _hubProxy;

        public SignalRManager(string url, string hubName)
        {
            Connection(url);
            CreateHubProxy(hubName);
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            await _hubConn.Start();
        }


        public void Hello(string message)
        {
            _hubProxy.Invoke("Send", message);
        }

        /// <summary>
        /// 连接signalr服务器
        /// </summary>
        /// <param name="url"></param>
        private void Connection(string url)
        {
            _hubConn = new HubConnection(url);
        }

        /// <summary>
        /// 创建hub代理
        /// </summary>
        /// <param name="hubName"></param>
        private void CreateHubProxy(string hubName)
        {
            _hubProxy = _hubConn.CreateHubProxy(hubName);
        }
    }
}