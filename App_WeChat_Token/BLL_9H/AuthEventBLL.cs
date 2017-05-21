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

        public string Receive(System.Net.Http.HttpRequestMessage request)
        {
            try
            {
                // appId 和 encodingAESKey 登录微信开放平台可见，通过中控器保证 appSecret 安全
                string appId = ConfigHelper.AppId;
                string appSecret = ConfigHelper.AppSecret;
                string encodingAESKey = ConfigHelper.EncodingAESKey;

                #region 1、推送component_verify_ticket协议
                // 1、推送component_verify_ticket协议
                Stream requestStream = Stream.Null;
                HttpContent content = request.Content;
                Task readTask = content.ReadAsStreamAsync().ContinueWith((task) => { requestStream = task.Result; });
                readTask.Wait();

                string requestBody_Cipher = string.Empty;
                using (StreamReader reader = new StreamReader(requestStream))
                {
                    requestBody_Cipher = reader.ReadToEnd();
                }

                // 记录requestBody_Cipher（XML格式）
                LogHelper.Info("1、推送component_verify_ticket协议" + "\r\n\r\n" + requestBody_Cipher);

                XmlNode root_Cipher = XmlHelper.Deserialize(requestBody_Cipher);
                string appId_Cipher = root_Cipher["AppId"].InnerText;

                // 判断 appId_Cipher == appId， 避免无效的异常报错，不一致无需解码

                string encrypt_Cipher = root_Cipher["Encrypt"].InnerText;
                string requestBody_Plain = Tencent.Cryptography.AES_decrypt(encrypt_Cipher, encodingAESKey, ref appId_Cipher);

                // 记录记录requestBody_Plain（XML格式）
                LogHelper.Info("1、推送component_verify_ticket协议" + "\r\n\r\n" + requestBody_Cipher + "\r\n\r\n" + requestBody_Plain);

                XmlNode root_Plain = XmlHelper.Deserialize(requestBody_Plain);
                string appId_Plain = root_Plain["AppId"].InnerText;

                // 判断 appId_Plain == appId， 避免无效的异常报错，不一致无需更新数据库

                string createTime_Plain = root_Plain["CreateTime"].InnerText;
                string infoType_Plain = root_Plain["InfoType"].InnerText; 
                #endregion

                if (infoType_Plain == "component_verify_ticket")
                {
                    #region component_verify_ticket
                    string componentVerifyTicket_Plain = root_Plain["ComponentVerifyTicket"].InnerText;

                    // component_verify_ticket每次更新， component_verify_ticket、component_access_token、pre_auth_code的 update_time设置为同一个值，可以减少数据库查询两次
                    DateTime dt = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local);
                    DateTime update_time = dt.AddSeconds(createTime_Plain.ToLong());
                    configDAL.Update("component_verify_ticket", componentVerifyTicket_Plain, update_time);

                    string component_access_token = null;

                    ConfigModel configModel = configDAL.GetModel("component_access_token");

                    component_access_token = configModel.Value;

                    int timestamp = (int)((DateTime.Now - configModel.Update_Time).TotalMinutes);
                    if (timestamp >= 110)
                    {
                        #region 2、获取第三方平台component_access_token
                        // 2、获取第三方平台component_access_token
                        component_access_token_req cat_req = new component_access_token_req();
                        cat_req.Component_AppId = appId;// 用自己的和传回来的都可以
                        cat_req.Component_AppSecret = appSecret;
                        cat_req.Component_Verify_Ticket = componentVerifyTicket_Plain;
                        string requestBody_2 = JsonConvert.SerializeObject(cat_req);

                        LogHelper.Info("2、获取第三方平台component_access_token" + "\r\n\r\n" + requestBody_2);

                        string responseBody_2 = HttpHelper.Post("https://api.weixin.qq.com/cgi-bin/component/api_component_token", requestBody_2);

                        LogHelper.Info("2、获取第三方平台component_access_token" + "\r\n\r\n" + requestBody_2 + "\r\n\r\n" + responseBody_2);

                        if (!string.IsNullOrEmpty(responseBody_2))
                        {
                            Component_Access_Token_Resp cat_resp = JsonConvert.DeserializeObject<Component_Access_Token_Resp>(responseBody_2);
                            if (cat_resp != null)
                            {
                                component_access_token = cat_resp.Component_Access_Token;

                                // component_access_token每1个小时50分钟更新一次
                                configDAL.Update("component_access_token", cat_resp.Component_Access_Token, update_time);
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                else if (infoType_Plain == "authorized")
                {
                    string authorizerAppid = root_Plain["AuthorizerAppid"].InnerText;
                    string authorizationCode = root_Plain["AuthorizationCode"].InnerText;
                    string authorizationCodeExpiredTime = root_Plain["AuthorizationCodeExpiredTime"].InnerText;
                }
                else if (infoType_Plain == "updateauthorized")
                {
                    string authorizerAppid = root_Plain["AuthorizerAppid"].InnerText;
                    string authorizationCode = root_Plain["AuthorizationCode"].InnerText;
                    string authorizationCodeExpiredTime = root_Plain["AuthorizationCodeExpiredTime"].InnerText;
                }
                else if (infoType_Plain == "unauthorized")
                {
                    string authorizerAppid = root_Plain["AuthorizerAppid"].InnerText;
                }

                return "success";
            }
            catch (Exception ex)
            {
                LogHelper.Error("唐群", ex);
                return "exception";
            }
        }
    }
}
