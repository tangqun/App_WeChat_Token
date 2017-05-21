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
        private IAuthInfoDAL authInfoDAL = new AuthInfoDAL();
        private ICodeMsgDAL codeMsgDAL = new CodeMsgDAL();

        public Model_9H.RESTfulModel Get(string authorizer_appid)
        {
            try
            {
                AuthInfoModel authInfoModel = authInfoDAL.GetModel(authorizer_appid);
                if (authInfoModel != null)
                {
                    int timestamp = (int)((DateTime.Now - authInfoModel.Refresh_Time).TotalMinutes);
                    if (timestamp >= 110)
                    {
                        ConfigModel configModel = configDAL.GetModel("component_access_token");

                        Authorizer_Access_Token_Req aat_req = new Authorizer_Access_Token_Req();
                        aat_req.Component_AppId = ConfigHelper.AppId;
                        aat_req.Authorizer_AppId = authInfoModel.Authorizer_Appid;
                        aat_req.Authorizer_Refresh_Token = authInfoModel.Authorizer_Refresh_Token;
                        string requestBody_5 = JsonConvert.SerializeObject(aat_req);

                        LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌）" + "\r\n\r\n" + requestBody_5);

                        string responseBody_5 = HttpHelper.Post("https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token=" + configModel.Value, requestBody_5);

                        LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌）" + "\r\n\r\n" + requestBody_5 + "\r\n\r\n" + responseBody_5);

                        Authorizer_Access_Token_Resp aat_resp = JsonConvert.DeserializeObject<Authorizer_Access_Token_Resp>(responseBody_5);
                        authInfoDAL.Refresh(authInfoModel.Authorizer_Appid, authInfoModel.Authorizer_Access_Token, aat_resp.Authorizer_Access_Token, aat_resp.Expires_In, aat_resp.Authorizer_Refresh_Token, DateTime.Now);

                        return new RESTfulModel() { Code = (int)CodeEnum.成功, Msg = string.Format(codeMsgDAL.GetByCode((int)CodeEnum.成功), "成功"), Data = aat_resp.Authorizer_Access_Token };
                    }
                    else
                    {
                        return new RESTfulModel() { Code = (int)CodeEnum.成功, Msg = string.Format(codeMsgDAL.GetByCode((int)CodeEnum.成功), "成功"), Data = authInfoModel.Authorizer_Access_Token };
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

            }
        }

        public void Refresh(object state)
        {
            List<AuthInfoModel> authInfoModelList = authInfoDAL.GetRefreshList();
            if (authInfoModelList.Any())
            {
                ConfigModel configModel = configDAL.GetModel("component_access_token");

                foreach (var authInfoModel in authInfoModelList)
                {
                    Authorizer_Access_Token_Req aat_req = new Authorizer_Access_Token_Req();
                    aat_req.Component_AppId = ConfigHelper.AppId;
                    aat_req.Authorizer_AppId = authInfoModel.Authorizer_Appid;
                    aat_req.Authorizer_Refresh_Token = authInfoModel.Authorizer_Refresh_Token;
                    string requestBody_5 = JsonConvert.SerializeObject(aat_req);

                    LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌）" + "\r\n\r\n" + requestBody_5);

                    string responseBody_5 = HttpHelper.Post("https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token=" + configModel.Value, requestBody_5);

                    LogHelper.Info("5、获取（刷新）授权公众号的接口调用凭据（令牌）" + "\r\n\r\n" + requestBody_5 + "\r\n\r\n" + responseBody_5);

                    Authorizer_Access_Token_Resp aat_resp = JsonConvert.DeserializeObject<Authorizer_Access_Token_Resp>(responseBody_5);
                    authInfoDAL.Refresh(authInfoModel.Authorizer_Appid, authInfoModel.Authorizer_Access_Token, aat_resp.Authorizer_Access_Token, aat_resp.Expires_In, aat_resp.Authorizer_Refresh_Token, DateTime.Now);
                }

                LogHelper.Info("监控中，" + authInfoModelList.Count + "个令牌已更新，如下：\r\n" + string.Join("\r\n", authInfoModelList.Select(o => o.Authorizer_Appid)));
            }
            else
            {
                LogHelper.Info("监控中，未有令牌即将过期");
            }
        }
    }
}
