using Helper_9H;
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
    public class AuthInfoDAL : IAuthInfoDAL
    {
        public List<AuthInfoModel> GetRefreshList()
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
            DataTable dt = MySqlHelper.ExecuteDataset(ConfigHelper.ConnStr, sql).Tables[0];
            return EntityListToModelList(dt);
        }

        public AuthInfoModel GetModel(string authorizer_appid)
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
            DataRow dr = MySqlHelper.ExecuteDataRow(ConfigHelper.ConnStr, sql, new MySqlParameter("@authorizer_appid", authorizer_appid));
            return EntityToModel(dr);
        }

        public bool Refresh(string authorizer_appid, string authorizer_access_token_old, string authorizer_access_token, int expires_in, string authorizer_refresh_token, DateTime refresh_time)
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
                                              new MySqlParameter("@authorizer_access_token_old", authorizer_access_token_old),
                                              new MySqlParameter("@authorizer_access_token", authorizer_access_token),
                                              new MySqlParameter("@expires_in", expires_in),
                                              new MySqlParameter("@authorizer_refresh_token", authorizer_refresh_token),
                                              new MySqlParameter("@refresh_time", refresh_time),
                                              new MySqlParameter("@update_time", refresh_time),
                                              new MySqlParameter("@authorizer_appid", authorizer_appid)
                                          };
            return MySqlHelper.ExecuteNonQuery(ConfigHelper.ConnStr, sql, parameters) > 0;
        }

        private List<AuthInfoModel> EntityListToModelList(DataTable dt)
        {
            List<AuthInfoModel> modelList = new List<AuthInfoModel>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    modelList.Add(EntityToModel(dr));
                }
            }
            return modelList;
        }

        private AuthInfoModel EntityToModel(DataRow dr)
        {
            if (dr != null)
            {
                AuthInfoModel model = new AuthInfoModel();
                model.Id = dr["id"].ToInt();
                model.Authorizer_Appid = dr["authorizer_appid"].ToString();
                model.Authorizer_Access_Token_Old = dr["authorizer_access_token_old"].ToString();
                model.Authorizer_Access_Token = dr["authorizer_access_token"].ToString();
                model.Expires_In = dr["expires_in"].ToInt();
                model.Authorizer_Refresh_Token = dr["authorizer_refresh_token"].ToString();
                model.Auth_Time = dr["auth_time"].ToDateTime();
                model.Refresh_Time = dr["refresh_time"].ToDateTime();
                model.Create_Time = dr["create_time"].ToDateTime();
                model.Update_Time = dr["update_time"].ToDateTime();
                return model;
            }
            return null;
        }
    }
}
