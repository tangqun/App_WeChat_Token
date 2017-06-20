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
    public class ConfigDAL : IConfigDAL
    {
        public Model_9H.ConfigModel GetModel(string key)
        {
            string sql =
                        @"SELECT
                            `id`,
                            `key`,
                            `value`,
                            `create_time`,
                            `update_time`
                        FROM `config`
                        WHERE `key` = @key
                        LIMIT 0, 1;";
            DataRow dr = MySqlHelper.ExecuteDataRow(ConfigHelper.ConnStr, sql, new MySqlParameter("@key", key));
            return EntityToModel(dr);
        }

        public bool Update(string key, string value, DateTime updateTime)
        {
            string sql =
                        @"UPDATE `config`
                        SET `value` = @value,
                            `update_time` = @update_time
                        WHERE `key` = @key;";
            MySqlParameter[] parameters = { 
                                              new MySqlParameter("@value", value),
                                              new MySqlParameter("@update_time", updateTime),
                                              new MySqlParameter("@key", key)
                                          };
            return MySqlHelper.ExecuteNonQuery(ConfigHelper.ConnStr, sql, parameters) > 0;
        }

        private Model_9H.ConfigModel EntityToModel(DataRow dr)
        {
            if (dr != null)
            {
                ConfigModel model = new ConfigModel();
                model.ID = dr["id"].ToInt();
                model.Key = dr["key"].ToString();
                model.Value = dr["value"].ToString();
                model.CreateTime = dr["create_time"].ToDateTime();
                model.UpdateTime = dr["update_time"].ToDateTime();
                return model;
            }
            return null;
        }
    }
}
