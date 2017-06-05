using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace progress.demo
{
    [HubName("progressTip")]
    public class ProgressTipHub : Hub
    {
        public void PushProgress(int progress, string timeSpent)
        {
            Clients.All.updateProgress(progress, timeSpent);
        }
    }
}