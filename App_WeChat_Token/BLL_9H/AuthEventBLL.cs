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

        public string Receive(Stream requestStream)
        {
            try
            {
                // appID 和 encodingAESKey 登录微信开放平台可见，通过中控器保证 appSecret 安全
                string componentAppID = ConfigHelper.ComponentAppID;
                string encodingAESKey = ConfigHelper.EncodingAESKey;

                #region 1、推送component_verify_ticket协议
                // 1、推送component_verify_ticket协议

                //<xml>
                //    <AppId><![CDATA[wxf07d20fb51c8d917]]></AppId>
                //    <Encrypt><![CDATA[CDquWt3mu0NqrLaD1+qYq/fUgjyGUNMBt13BR/Ubh6U2yYwkuuC7EtySesZRbKvbkB8XS6HkLay0/QnGi/IGU9pm+k2QJ27zmkdKC7y78cjYxG3/YZ/l0+kG01yTqtaEOeIvkvIyJ+RMX9S4z2fNLJ/FynXZoYtrqAkniR0OaZ4tk3P5/1f9CeXsEb5OxOc+piY8micnnTBXFHGYgd7xv5CF+5HMJauIksjaAVYqFTvJ/nCQ2K60pyVwJHyaWz5Kcroso3JdqOoQA7J5/2oyKpfZK0Ymi6U0aA4kNsPwMzH4XJ22cP7FAstm/TyLinLVmz1AHrieuxZU9o7w6kGscm+niNoaJOOWY/zgVcuEpBH0rrLNTUxzjNW7PYvmYoL3OEfib0cH2V7IDoTlt8GTbnaigcL+SEIbGrPQG8BBv7zlO1glMS8Lcp3xMqUsImtGgQ5B7aCaJGUI0MOqKN+71w==]]></Encrypt>
                //</xml>

                string requestBody = string.Empty;
                using (StreamReader reader = new StreamReader(requestStream))
                {
                    requestBody = reader.ReadToEnd();
                }

                // 记录requestBody（XML格式）
                LogHelper.Info("1、推送component_verify_ticket协议" + "\r\n\r\nrequestBody: " + requestBody);

                XmlNode root = XmlHelper.Deserialize(requestBody);

                // 用自己的AppID去解码
                string encryptText = root["Encrypt"].InnerText;
                string decryptedRequestBody = Tencent.Cryptography.AES_decrypt(encryptText, encodingAESKey, ref componentAppID);

                //<xml>
                //	<AppId><![CDATA[wxf07d20fb51c8d917]]></AppId>
                //	<CreateTime>1495382674</CreateTime>
                //	<InfoType><![CDATA[component_verify_ticket]]></InfoType>
                //	<ComponentVerifyTicket><![CDATA[ticket@@@P3O_Id4Yj6p7ihohpImQwcGA8nS3WXf745wEeIqZIqJNB6uTqr9fLavLXZ7roC9vpASZLK5QVPfuoMGR95iJqA]]></ComponentVerifyTicket>
                //</xml>

                // 记录记录decryptedRequestBody（XML格式）
                LogHelper.Info("1、推送component_verify_ticket协议" + "\r\n\r\nrequestBody: " + requestBody + "\r\n\r\ndecryptedRequestBody: " + decryptedRequestBody);

                XmlNode decryptedRoot = XmlHelper.Deserialize(decryptedRequestBody);
                string decryptedAppIDText = decryptedRoot["AppId"].InnerText;
                string decryptedCreateTimeText = decryptedRoot["CreateTime"].InnerText;
                string decryptedInfoTypeText = decryptedRoot["InfoType"].InnerText; 
                #endregion

                if (decryptedInfoTypeText == "component_verify_ticket")
                {
                    #region component_verify_ticket
                    string decryptedComponentVerifyTicketText = decryptedRoot["ComponentVerifyTicket"].InnerText;

                    // component_verify_ticket每次更新， component_verify_ticket、component_access_token的 update_time设置为同一个值，可以减少数据库查询两次
                    DateTime dt = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local);
                    DateTime updateTime = dt.AddSeconds(decryptedCreateTimeText.ToLong());

                    configDAL.Update("component_verify_ticket", decryptedComponentVerifyTicketText, updateTime);

                    UpdateComponentAccessToken(decryptedComponentVerifyTicketText, updateTime);
                    #endregion
                }
                else if (decryptedInfoTypeText == "authorized")
                {
                    //string authorizerAppid = root_Plain["AuthorizerAppid"].InnerText;
                    //string authorizationCode = root_Plain["AuthorizationCode"].InnerText;
                    //string authorizationCodeExpiredTime = root_Plain["AuthorizationCodeExpiredTime"].InnerText;
                }
                else if (decryptedInfoTypeText == "updateauthorized")
                {
                    //string authorizerAppid = root_Plain["AuthorizerAppid"].InnerText;
                    //string authorizationCode = root_Plain["AuthorizationCode"].InnerText;
                    //string authorizationCodeExpiredTime = root_Plain["AuthorizationCodeExpiredTime"].InnerText;
                }
                else if (decryptedInfoTypeText == "unauthorized")
                {
                    //string authorizerAppid = root_Plain["AuthorizerAppid"].InnerText;
                }

                return "success";
            }
            catch (Exception ex)
            {
                LogHelper.Error("唐群", ex);
                return "exception";
            }
        }

        private void UpdateComponentAccessToken(string componentVerifyTicket, DateTime updateTime)
        {
            string componentAppID = ConfigHelper.ComponentAppID;
            string componentAppSecret = ConfigHelper.ComponentAppSecret;

            ConfigModel configModel = configDAL.GetModel("component_access_token");
            int timestamp = (int)((DateTime.Now - configModel.UpdateTime).TotalMinutes);
            if (timestamp >= 110)
            {
                #region 2、获取第三方平台component_access_token
                // 2、获取第三方平台component_access_token
                ComponentAccessTokenGetReq req = new ComponentAccessTokenGetReq();
                req.ComponentAppId = componentAppID;// 用自己的和传回来的都可以
                req.ComponentAppSecret = componentAppSecret;
                req.ComponentVerifyTicket = componentVerifyTicket;
                string requestBody = JsonConvert.SerializeObject(req);

                LogHelper.Info("2、获取第三方平台component_access_token" + "\r\n\r\nrequestBody: " + requestBody);

                string responseBody = HttpHelper.Post("https://api.weixin.qq.com/cgi-bin/component/api_component_token", requestBody);

                LogHelper.Info("2、获取第三方平台component_access_token" + "\r\n\r\nrequestBody: " + requestBody + "\r\n\r\nresponseBody: " + responseBody);

                if (!string.IsNullOrEmpty(responseBody))
                {
                    ComponentAccessTokenGetResp resp = JsonConvert.DeserializeObject<ComponentAccessTokenGetResp>(responseBody);
                    if (resp != null)
                    {
                        // component_access_token每1个小时50分钟更新一次
                        configDAL.Update("component_access_token", resp.ComponentAccessToken, updateTime);
                    }
                }
                #endregion
            }
        }
    }
}
