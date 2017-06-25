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
using Newtonsoft.Json;

namespace BLL_9H
{
    public class AuthorizationInfoBLL : IAuthorizationInfoBLL
    {
        private ICodeMsgDAL codeMsgDAL = new CodeMsgDAL();
        private IAuthorizationInfoDAL authorizationInfoDAL = new AuthorizationInfoDAL();

        public RESTfulModel Save(SaveAuthModel model)
        {
            // 授权信息存数据库
            try
            {
                // 验证

                // jsapi_ticket
                string url_jsapi = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + model.AuthorizerAccessToken + "&type=jsapi";
                LogHelper.Info("获取（刷新）jsapi_ticket url_jsapi", url_jsapi);
                string responseBody_jsapi = HttpHelper.Get(url_jsapi);
                LogHelper.Info("获取（刷新）jsapi_ticket responseBody_jsapi", responseBody_jsapi);
                TicketGetResp resp_jsapi = JsonConvert.DeserializeObject<TicketGetResp>(responseBody_jsapi);

                // api_ticket
                string url_api = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + model.AuthorizerAccessToken + "&type=wx_card";
                LogHelper.Info("获取（刷新）api_ticket url_api", url_api);
                string responseBody_api = HttpHelper.Get(url_api);
                LogHelper.Info("获取（刷新）api_ticket responseBody_api", responseBody_api);
                TicketGetResp resp_api = JsonConvert.DeserializeObject<TicketGetResp>(responseBody_api);

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
                        resp_jsapi.Ticket, 
                        resp_api.Ticket,
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
                        resp_jsapi.Ticket,
                        resp_api.Ticket,
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
