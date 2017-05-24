﻿using Helper_9H;
using IDAL_9H;
using Model_9H;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_9H
{
    public class AuthorizationInfoDAL : IAuthorizationInfoDAL
    {
        public List<AuthorizationInfoModel> GetRefreshList()
        {
            string sql =
                        @"SELECT
                            `id`,
                            `authorizer_appid`,
                            `authorizer_access_token_old`,
                            `authorizer_access_token`,
                            `expires_in`,
                            `authorizer_refresh_token`,
                            `auth_time`,
                            `refresh_time`,
                            `create_time`,
                            `update_time`
                        FROM `authorization_info`
                        WHERE TIMESTAMPDIFF(SECOND, refresh_time, NOW()) + 600 >= expires_in;";
            DataTable dt = MySqlHelper.ExecuteDataset(ConfigHelper.ConnStr_jhwechat, sql).Tables[0];
            return EntityListToModelList(dt);
        }

        public AuthorizationInfoModel GetModel(string authorizerAppID)
        {
            string sql =
                        @"SELECT
                            `id`,
                            `authorizer_appid`,
                            `authorizer_access_token_old`,
                            `authorizer_access_token`,
                            `expires_in`,
                            `authorizer_refresh_token`,
                            `auth_time`,
                            `refresh_time`,
                            `create_time`,
                            `update_time`
                        FROM `authorization_info`
                        WHERE `authorizer_appid` = @authorizer_appid
                        LIMIT 0, 1;";
            DataRow dr = MySqlHelper.ExecuteDataRow(ConfigHelper.ConnStr_jhwechat, sql, new MySqlParameter("@authorizer_appid", authorizerAppID));
            return EntityToModel(dr);
        }

        public bool Refresh(string authorizerAppID, string authorizerAccessTokenOld, string authorizerAccessToken, int expiresIn, string authorizerRefreshToken, DateTime refreshTime)
        {
            string sql =
                        @"UPDATE `authorization_info`
                        SET `authorizer_access_token_old` = @authorizer_access_token_old,
                            `authorizer_access_token` = @authorizer_access_token,
                            `expires_in` = @expires_in,
                            `authorizer_refresh_token` = @authorizer_refresh_token,
                            `refresh_time` = @refresh_time,
                            `update_time` = @update_time
                        WHERE `authorizer_appid` = @authorizer_appid;";
            MySqlParameter[] parameters = { 
                                              new MySqlParameter("@authorizer_access_token_old", authorizerAccessTokenOld),
                                              new MySqlParameter("@authorizer_access_token", authorizerAccessToken),
                                              new MySqlParameter("@expires_in", expiresIn),
                                              new MySqlParameter("@authorizer_refresh_token", authorizerRefreshToken),
                                              new MySqlParameter("@refresh_time", refreshTime),
                                              new MySqlParameter("@update_time", refreshTime),
                                              new MySqlParameter("@authorizer_appid", authorizerAppID)
                                          };
            return MySqlHelper.ExecuteNonQuery(ConfigHelper.ConnStr_jhwechat, sql, parameters) > 0;
        }

        private List<AuthorizationInfoModel> EntityListToModelList(DataTable dt)
        {
            List<AuthorizationInfoModel> modelList = new List<AuthorizationInfoModel>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    modelList.Add(EntityToModel(dr));
                }
            }
            return modelList;
        }

        private AuthorizationInfoModel EntityToModel(DataRow dr)
        {
            if (dr != null)
            {
                AuthorizationInfoModel model = new AuthorizationInfoModel();
                model.ID = dr["id"].ToInt();
                model.AuthorizerAppID = dr["authorizer_appid"].ToString();
                model.AuthorizerAccessTokenOld = dr["authorizer_access_token_old"].ToString();
                model.AuthorizerAccessToken = dr["authorizer_access_token"].ToString();
                model.ExpiresIn = dr["expires_in"].ToInt();
                model.AuthorizerRefreshToken = dr["authorizer_refresh_token"].ToString();
                model.AuthTime = dr["auth_time"].ToDateTime();
                model.RefreshTime = dr["refresh_time"].ToDateTime();
                model.CreateTime = dr["create_time"].ToDateTime();
                model.UpdateTime = dr["update_time"].ToDateTime();
                return model;
            }
            return null;
        }
    }
}
