using Model_9H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL_9H
{
    public interface IAuthorizationInfoDAL
    {
        List<AuthorizationInfoModel> GetRefreshList();

        AuthorizationInfoModel GetModel(string authorizerAppID);

        // 刷新令牌
        bool Refresh(string authorizerAppID, string authorizerAccessTokenOld, string authorizerAccessToken, int expiresIn, string authorizerRefreshToken, DateTime refreshTime);
    }
}
