using Model_9H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL_9H
{
    public interface IAuthInfoDAL
    {
        List<AuthInfoModel> GetRefreshList();

        AuthInfoModel GetModel(string authorizer_appid);

        // 刷新令牌
        bool Refresh(string authorizer_appid, string authorizer_access_token_old, string authorizer_access_token, int expires_in, string authorizer_refresh_token, DateTime refresh_time);
    }
}
