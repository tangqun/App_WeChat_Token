using IBLL_9H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model_9H;
using IDAL_9H;
using DAL_9H;
using Helper_9H;

namespace BLL_9H
{
    public class AuthorizationInfoBLL : IAuthorizationInfoBLL
    {
        private ICodeMsgDAL codeMsgDAL = new CodeMsgDAL();
        private IAuthorizationInfoDAL authorizationInfoDAL = new AuthorizationInfoDAL();

        public RESTfulModel Save(AuthModel model)
        {
            // 授权信息存数据库
            try
            {
                // 验证

                AuthorizationInfoModel authorizationInfoModel = authorizationInfoDAL.GetModel(model.AuthorizerAppID);
                if (authorizationInfoModel != null)
                {
                    // 更新
                    authorizationInfoDAL.Update(
                        model.AuthorizerAppID,
                        authorizationInfoModel.AuthorizerAccessToken,// 当前的置为旧的，用于消息延时
                        model.AuthorizerAccessToken,
                        model.ExpiresIn,
                        model.AuthorizerRefreshToken,
                        model.AuthTime);
                }
                else
                {
                    // 插入
                    authorizationInfoDAL.Insert(
                        model.AuthorizerAppID,
                        model.AuthorizerAccessToken,
                        model.AuthorizerAccessToken,
                        model.ExpiresIn,
                        model.AuthorizerRefreshToken,
                        model.AuthTime);
                }

                return new RESTfulModel() { Code = (int)CodeEnum.成功, Msg = string.Format(codeMsgDAL.GetByCode((int)CodeEnum.成功), "成功") };
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return new RESTfulModel() { Code = (int)CodeEnum.系统异常, Msg = codeMsgDAL.GetByCode((int)CodeEnum.系统异常) };
            }
        }
    }
}
