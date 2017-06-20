using DAL_9H;
using Helper_9H;
using IBLL_9H;
using IDAL_9H;
using Model_9H;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BLL_9H
{
    public class AuthEventBLL : IAuthEventBLL
    {
        private IConfigDAL configDAL = new ConfigDAL();

        public string Receive(string requestBody_Cipher)
        {
            try
            {
                // AppID和EncodingAESKey登录微信开放平台可见，通过中控器保证AppSecret安全
                string appID = ConfigHelper.AppID;
                string appSecret = ConfigHelper.AppSecret;
                string encodingAESKey = ConfigHelper.EncodingAESKey;

                #region 1、推送component_verify_ticket协议
                // 1、推送component_verify_ticket协议
                //Stream requestStream = Stream.Null;
                //HttpContent content = request.Content;
                //Task readTask = content.ReadAsStreamAsync().ContinueWith((task) => { requestStream = task.Result; });
                //readTask.Wait();

                //string requestBody_Cipher = string.Empty;
                //using (StreamReader reader = new StreamReader(requestStream))
                //{
                //    requestBody_Cipher = reader.ReadToEnd();
                //}

                // 记录requestBody_Cipher（XML格式）
                LogHelper.Info("1、推送component_verify_ticket协议 requestBody_Cipher", requestBody_Cipher);

                XmlNode root_Cipher = XmlHelper.Deserialize(requestBody_Cipher);
                string encrypt_Cipher = root_Cipher["Encrypt"].InnerText;
                string requestBody = Tencent.Cryptography.AES_decrypt(encrypt_Cipher, encodingAESKey, ref appID);

                // 记录requestBody（XML格式）
                LogHelper.Info("1、推送component_verify_ticket协议 requestBody", requestBody);

                XmlNode root = XmlHelper.Deserialize(requestBody);
                string createTime = root["CreateTime"].InnerText;
                string infoType = root["InfoType"].InnerText; 
                #endregion

                if (infoType == "component_verify_ticket")
                {
                    #region component_verify_ticket
                    string componentVerifyTicket = root["ComponentVerifyTicket"].InnerText;

                    // component_verify_ticket 每次更新， component_verify_ticket、component_access_token 的 update_time 设置为同一个值，可以减少数据库查询两次
                    DateTime dt = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local);
                    DateTime updateTime = dt.AddSeconds(createTime.ToLong());
                    configDAL.Update("component_verify_ticket", componentVerifyTicket, updateTime);
                    
                    ConfigModel configModel = configDAL.GetModel("component_access_token");

                    int timestamp = (int)((DateTime.Now - configModel.UpdateTime).TotalMinutes);
                    if (timestamp >= 110)
                    {
                        #region 2、获取第三方平台component_access_token
                        // 2、获取第三方平台component_access_token
                        string url = "https://api.weixin.qq.com/cgi-bin/component/api_component_token";

                        LogHelper.Info("2、获取第三方平台component_access_token url", url);

                        ComponentAccessTokenGetReq req_2 = new ComponentAccessTokenGetReq();
                        req_2.ComponentAppID = appID;
                        req_2.ComponentAppSecret = appSecret;
                        req_2.ComponentVerifyTicket = componentVerifyTicket;
                        string requestBody_2 = JsonConvert.SerializeObject(req_2);

                        LogHelper.Info("2、获取第三方平台component_access_token requestBody_2", requestBody_2);

                        string responseBody_2 = HttpHelper.Post(url, requestBody_2);

                        LogHelper.Info("2、获取第三方平台component_access_token responseBody_2", responseBody_2);

                        ComponentAccessTokenGetResp resp_2 = JsonConvert.DeserializeObject<ComponentAccessTokenGetResp>(responseBody_2);
                        // component_access_token每隔1小时50分钟更新一次
                        configDAL.Update("component_access_token", resp_2.ComponentAccessToken, updateTime);
                        #endregion
                    }
                    #endregion
                }
                else if (infoType == "authorized")
                {
                    string authorizerAppid = root["AuthorizerAppid"].InnerText;
                    string authorizationCode = root["AuthorizationCode"].InnerText;
                    string authorizationCodeExpiredTime = root["AuthorizationCodeExpiredTime"].InnerText;
                }
                else if (infoType == "updateauthorized")
                {
                    string authorizerAppid = root["AuthorizerAppid"].InnerText;
                    string authorizationCode = root["AuthorizationCode"].InnerText;
                    string authorizationCodeExpiredTime = root["AuthorizationCodeExpiredTime"].InnerText;
                }
                else if (infoType == "unauthorized")
                {
                    string authorizerAppid = root["AuthorizerAppid"].InnerText;
                }

                return "success";
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return "exception";
            }
        }
    }
}
