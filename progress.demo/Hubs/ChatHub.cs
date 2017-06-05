using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace progress.demo
{
    /// <summary>
    /// 聊天集线器
    /// </summary>
    public class ChatHub : Hub
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}