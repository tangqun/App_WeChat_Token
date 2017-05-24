using BLL_9H;
using Helper_9H;
using IBLL_9H;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api_9H.Controllers
{
    public class AuthEventController : ApiController
    {
        private IAuthEventBLL authEventBLL = new AuthEventBLL();

        public string Receive()
        {
            try
            {
                // ashx --> System.Web.HttpContext.Current.Request.InputStream
                // asp.net mvc --> Request.InputStream
                // asp.net --> Request.Content.ReadAsStreamAsync().ContinueWith((task) => { requestStream = task.Result; });
                // 方便模拟测试
                Stream requestStream = Stream.Null;
                Task readTask = Request.Content.ReadAsStreamAsync().ContinueWith((task) => { requestStream = task.Result; });
                readTask.Wait();

                return authEventBLL.Receive(requestStream);
            }
            catch (Exception ex)
            {
                LogHelper.Error("唐群", ex);
                return "exception";
            }
        }
    }
}
