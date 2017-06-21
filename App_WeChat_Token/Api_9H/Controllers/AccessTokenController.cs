using BLL_9H;
using IBLL_9H;
using Model_9H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Token_9H.Controllers
{
    public class AccessTokenController : ApiController
    {
        private IAccessTokenBLL accessTokenBLL = new AccessTokenBLL();
        private IAuthorizationInfoBLL authorizationInfoBLL = new AuthorizationInfoBLL();

        /// <summary>
        /// 获取
        /// </summary>
        [HttpGet]
        public RESTfulModel Get(string authorizerAppID)
        {
            return accessTokenBLL.Get(authorizerAppID);
        }

        /// <summary>
        /// 存储
        /// </summary>
        [HttpPost]
        public RESTfulModel Save(SaveAuthModel model)
        {
            return authorizationInfoBLL.Save(model);
        }
    }
}
