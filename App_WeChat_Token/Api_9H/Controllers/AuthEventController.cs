using BLL_9H;
using IBLL_9H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api_9H.Controllers
{
    public class AuthEventController : ApiController
    {
        private IAuthEventBLL authEventBLL = new AuthEventBLL();

        public string Receive()
        {
            return authEventBLL.Receive(Request.Content.ReadAsStringAsync().Result);
        }
    }
}
