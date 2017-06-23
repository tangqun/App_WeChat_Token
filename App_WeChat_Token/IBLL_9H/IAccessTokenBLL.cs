using Model_9H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IBLL_9H
{
    public interface IAccessTokenBLL
    {
        RESTfulModel Get(string authorizer_appid);

        void RefreshForAuthorized();
    }
}
