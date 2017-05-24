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
        private IConfigDAL configDAL = new ConfigDAL();
        private IAuthorizationInfoDAL authorizationInfoDAL = new AuthorizationInfoDAL();
        private ICodeMsgDAL codeMsgDAL = new CodeMsgDAL();

        public Model_9H.RESTfulModel Get(string authorizerAppID)
        {
            try
            {
                AuthorizationInfoModel authorizationInfoModel = authorizationInfoDAL.GetModel(authorizerAppID);
                if (authorizationInfoModel != null)
                {
                    int timestamp = (int)((DateTime.Now - authorizationInfoModel.RefreshTime).TotalMinutes);
                    if (timestamp >= 110)
                    {
                        string componentAppID = ConfigHelper.ComponentAppID;

                        ConfigModel configModel = configDAL.GetModel("component_access_token");

                        AuthorizerAccessTokenGetReq req = new AuthorizerAccessTokenGetReq();
                        req.ComponentAppID = componentAppID;
                        req.AuthorizerAppID = authorizationInfoModel.AuthorizerAppID;
                        req.AuthorizerRefreshToken = authorizationInfoModel.AuthorizerRefreshToken;
                        string requestBody = JsonConvert.SerializeObject(req);

                        LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌）" + "\r\n\r\nrequestBody: " + requestBody);

                        string responseBody = HttpHelper.Post("https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token=" + configModel.Value, requestBody);

                        LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌）" + "\r\n\r\nrequestBody: " + requestBody + "\r\n\r\nresponseBody: " + responseBody);

                        AuthorizerAccessTokenGetResp resp = JsonConvert.DeserializeObject<AuthorizerAccessTokenGetResp>(responseBody);

                        authorizationInfoDAL.Refresh(authorizationInfoModel.AuthorizerAppID, authorizationInfoModel.AuthorizerAccessToken, resp.AuthorizerAccessToken, resp.ExpiresIn, resp.AuthorizerRefreshToken, DateTime.Now);

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
                LogHelper.Error("唐群", ex);
                return new RESTfulModel() { Code = (int)CodeEnum.系统异常, Msg = codeMsgDAL.GetByCode((int)CodeEnum.系统异常) };
            }
        }

        public void Timer()
        {
            try
            {
                LogHelper.Info("定时器已经启动");
                Timer timer = new Timer(Refresh, null, 0, 1000 * 60 * 5);
            }
            catch (Exception ex)
            {
                LogHelper.Error("唐群", ex);
            }
        }

        public void Refresh(object state)
        {
            List<AuthorizationInfoModel> authorizationInfoModelList = authorizationInfoDAL.GetRefreshList();
            if (authorizationInfoModelList.Any())
            {
                string componenAppID = ConfigHelper.ComponentAppID;

                ConfigModel configModel = configDAL.GetModel("component_access_token");

                foreach (var authorizationInfoModel in authorizationInfoModelList)
                {
                    AuthorizerAccessTokenGetReq req = new AuthorizerAccessTokenGetReq();
                    req.ComponentAppID = componenAppID;
                    req.AuthorizerAppID = authorizationInfoModel.AuthorizerAppID;
                    req.AuthorizerRefreshToken = authorizationInfoModel.AuthorizerRefreshToken;
                    string requestBody = JsonConvert.SerializeObject(req);

                    LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌）" + "\r\n\r\nrequestBody: " + requestBody);

                    string responseBody = HttpHelper.Post("https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token=" + configModel.Value, requestBody);

                    LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌）" + "\r\n\r\nrequestBody: " + requestBody + "\r\n\r\nresponseBody: " + responseBody);

                    AuthorizerAccessTokenGetResp resp = JsonConvert.DeserializeObject<AuthorizerAccessTokenGetResp>(responseBody);
                    authorizationInfoDAL.Refresh(authorizationInfoModel.AuthorizerAppID, authorizationInfoModel.AuthorizerAccessToken, resp.AuthorizerAccessToken, resp.ExpiresIn, resp.AuthorizerRefreshToken, DateTime.Now);
                }

                LogHelper.Info("监控中，" + authorizationInfoModelList.Count + "个令牌已更新，如下：\r\n" + string.Join("\r\n", authorizationInfoModelList.Select(o => o.AuthorizerAppID)));
            }
            else
            {
                LogHelper.Info("监控中，未有令牌即将过期");
            }
        }
    }
}
