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
        private IAuthorizationInfoDAL authInfoDAL = new AuthorizationInfoDAL();

        public Model_9H.RESTfulModel Get(string authorizerAppID)
        {
            try
            {
                AuthorizationInfoModel authorizationInfoModel = authInfoDAL.GetModel(authorizerAppID);
                if (authorizationInfoModel != null)
                {
                    int timestamp = (int)((DateTime.Now - authorizationInfoModel.UpdateTime).TotalMinutes);
                    if (timestamp >= 110)
                    {
                        DateTime createTime = DateTime.Now;

                        ConfigModel configModel = configDAL.GetModel("component_access_token");
                        string url = "https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token=" + configModel.Value;

                        LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌） url", url);

                        AuthorizerAccessTokenGetReq req_5 = new AuthorizerAccessTokenGetReq();
                        req_5.ComponentAppID = ConfigHelper.AppID;
                        req_5.AuthorizerAppID = authorizationInfoModel.AuthorizerAppID;
                        req_5.AuthorizerRefreshToken = authorizationInfoModel.AuthorizerRefreshToken;
                        string requestBody_5 = JsonConvert.SerializeObject(req_5);

                        LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌） requestBody_5", requestBody_5);

                        string responseBody_5 = HttpHelper.Post(url, requestBody_5);

                        LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌） responseBody_5", responseBody_5);

                        AuthorizerAccessTokenGetResp resp = JsonConvert.DeserializeObject<AuthorizerAccessTokenGetResp>(responseBody_5);
                        authInfoDAL.Refresh(authorizationInfoModel.AuthorizerAppID, authorizationInfoModel.AuthorizerAccessToken, resp.AuthorizerAccessToken, resp.ExpiresIn, resp.AuthorizerRefreshToken, createTime);

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

        //public void Refresh(object state)
        //{
        //    List<AuthorizationInfoModel> authInfoModelList = authInfoDAL.GetRefreshList();
        //    if (authInfoModelList.Any())
        //    {
        //        ConfigModel configModel = configDAL.GetModel("component_access_token");

        //        foreach (var authInfoModel in authInfoModelList)
        //        {
        //            Authorizer_Access_Token_Req aat_req = new Authorizer_Access_Token_Req();
        //            aat_req.Component_AppId = ConfigHelper.AppID;
        //            aat_req.Authorizer_AppId = authInfoModel.Authorizer_Appid;
        //            aat_req.Authorizer_Refresh_Token = authInfoModel.Authorizer_Refresh_Token;
        //            string requestBody_5 = JsonConvert.SerializeObject(aat_req);

        //            LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌）" + "\r\n\r\n" + requestBody_5);

        //            string responseBody_5 = HttpHelper.Post("https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token=" + configModel.Value, requestBody_5);

        //            LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌）" + "\r\n\r\n" + requestBody_5 + "\r\n\r\n" + responseBody_5);

        //            Authorizer_Access_Token_Resp aat_resp = JsonConvert.DeserializeObject<Authorizer_Access_Token_Resp>(responseBody_5);
        //            authInfoDAL.Refresh(authInfoModel.Authorizer_Appid, authInfoModel.Authorizer_Access_Token, aat_resp.Authorizer_Access_Token, aat_resp.Expires_In, aat_resp.Authorizer_Refresh_Token, DateTime.Now);
        //        }

        //        LogHelper.Info("监控中，" + authInfoModelList.Count + "个令牌已更新，如下：\r\n" + string.Join("\r\n", authInfoModelList.Select(o => o.Authorizer_Appid)));
        //    }
        //    else
        //    {
        //        LogHelper.Info("监控中，未有令牌即将过期");
        //    }
        //}
    }
}
