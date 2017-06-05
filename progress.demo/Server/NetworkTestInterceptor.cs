using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace progress.demo
{
    public class NetworkTestInterceptor : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //获取URL参数  
            NameValueCollection query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            //获取Post正文数据，比如json文本  
            string fRequesContent = request.Content.ReadAsStringAsync().Result;

            //可以做一些其他安全验证工作，比如Token验证，签名验证。  
            //可以在需要时自定义HTTP响应消息  
            //return SendError("自定义的HTTP响应消息", HttpStatusCode.OK);  

            //请求处理耗时跟踪  
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //调用内部处理接口，并获取HTTP响应消息  
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            sw.Stop();
            //记录处理耗时  
            long exeMs = sw.ElapsedMilliseconds;
            return response;
        }
    }
}