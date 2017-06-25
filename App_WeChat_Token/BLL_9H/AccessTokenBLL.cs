using DAL_9H;
using Helper_9H;
using IBLL_9H;
using IDAL_9H;
using Model_9H;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL_9H
{
    public class AccessTokenBLL : IAccessTokenBLL
    {
        private ICodeMsgDAL codeMsgDAL = new CodeMsgDAL();
        private IConfigDAL configDAL = new ConfigDAL();
        private IAuthorizationInfoDAL authorizationInfoDAL = new AuthorizationInfoDAL();

        public Model_9H.RESTfulModel Get(string authorizerAppID)
        {
            try
            {
                AuthorizationInfoModel authorizationInfoModel = authorizationInfoDAL.GetModel(authorizerAppID);
                if (authorizationInfoModel != null)
                {
                    int timestamp = (int)((DateTime.Now - authorizationInfoModel.UpdateTime).TotalMinutes);
                    if (timestamp >= 110)
                    {
                        ConfigModel configModel = configDAL.GetModel("component_access_token");
                        string url_5 = "https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token=" + configModel.Value;

                        LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌） url_5", url_5);

                        AuthorizerAccessTokenGetResp resp = Refresh(authorizationInfoModel.AuthorizerAppID, authorizationInfoModel.AuthorizerAccessToken, configModel.Value);

                        return new RESTfulModel() { Code = (int)CodeEnum.成功, Msg = string.Format(codeMsgDAL.GetByCode((int)CodeEnum.成功), "成功"), Data = resp.AuthorizerAccessToken };
                    }
                    else
                    {
                        return new RESTfulModel() { Code = (int)CodeEnum.成功, Msg = string.Format(codeMsgDAL.GetByCode((int)CodeEnum.成功), "成功"), Data = authorizationInfoModel.AuthorizerAccessToken };
                    }
                }
                else
                {
                    return new RESTfulModel() { Code = (int)CodeEnum.用户未授权, Msg = codeMsgDAL.GetByCode((int)CodeEnum.用户未授权) };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return new RESTfulModel() { Code = (int)CodeEnum.系统异常, Msg = codeMsgDAL.GetByCode((int)CodeEnum.系统异常) };
            }
        }

        private AuthorizerAccessTokenGetResp Refresh(string authorizerAppID, string authorizerRefreshToken, string componentAccessToken)
        {
            DateTime refreshTime = DateTime.Now;

            string url_5 = "https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token=" + componentAccessToken;

            LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌） url_5", url_5);

            AuthorizerAccessTokenGetReq req_5 = new AuthorizerAccessTokenGetReq()
            {
                ComponentAppID = ConfigHelper.ComponentAppID,
                AuthorizerAppID = authorizerAppID,
                AuthorizerRefreshToken = authorizerRefreshToken
            };
            string requestBody_5 = JsonConvert.SerializeObject(req_5);

            LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌） requestBody_5", requestBody_5);

            string responseBody_5 = HttpHelper.Post(url_5, requestBody_5);

            LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌） responseBody_5", responseBody_5);

            AuthorizerAccessTokenGetResp resp = JsonConvert.DeserializeObject<AuthorizerAccessTokenGetResp>(responseBody_5);
            authorizationInfoDAL.Refresh(authorizerAppID, authorizerRefreshToken /* old */, resp.AuthorizerAccessToken, resp.ExpiresIn, resp.AuthorizerRefreshToken, refreshTime);
            return resp;
        }

        public void RefreshForAuthorized()
        {
            try
            {
                List<AuthorizationInfoModel> authorizationInfoModelList = authorizationInfoDAL.GetRefreshList();
                if (authorizationInfoModelList.Any())
                {
                    ConfigModel configModel = configDAL.GetModel("component_access_token");

                    foreach (var authorizationInfoModel in authorizationInfoModelList)
                    {
                        Refresh(authorizationInfoModel.AuthorizerAppID, authorizationInfoModel.AuthorizerRefreshToken, configModel.Value);
                    }

                    LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌） 监控中...", "【 " + authorizationInfoModelList.Count + " 】个令牌已更新，如下：\r\n" + string.Join("\r\n", authorizationInfoModelList.Select(o => o.AuthorizerAppID)));
                }
                else
                {
                    LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌） 监控中...", "未有令牌即将过期");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }
    }
}
