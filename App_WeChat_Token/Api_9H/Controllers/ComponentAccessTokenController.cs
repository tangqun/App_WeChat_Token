using BLL_9H;
using IBLL_9H;
using Model_9H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api_9H.Controllers
{
    public class ComponentAccessTokenController : ApiController
    {
        private IComponentAccessTokenBLL componentAccessTokenBLL = new ComponentAccessTokenBLL();

        public RESTfulModel Get()
        {
            return componentAccessTokenBLL.Get();
        }
    }
}
