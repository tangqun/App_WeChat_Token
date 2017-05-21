﻿using Helper_9H;
using IDAL_9H;
using Model_9H;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_9H
{
    public class CodeMsgDAL : ICodeMsgDAL
    {
        #region 前台
        public string GetByCode(int code)
        {
            CodeMsgModel retModel = Singleton_CodeMsg.GetInstance().Where(o => o.code == code).Single();
            return retModel == null ? "" : retModel.msg;
        }
        #endregion
    }

    public sealed class Singleton_CodeMsg
    {
        // 懒汉式单例模式
        // 存在内存中效率更高
        // 每次添加或修改返回码需要改代码，所以可以用此种方式加载返回码
        private static List<CodeMsgModel> instance = new List<CodeMsgModel>() { 

            // 系统异常
            new CodeMsgModel(){ code = (int)CodeEnum.系统异常, msg = "系统异常" },
            // 成功
            new CodeMsgModel(){ code = (int)CodeEnum.成功, msg = "{0}" },
            // 失败
            new CodeMsgModel(){ code = (int)CodeEnum.失败, msg = "{0}" },

            // Web 注册登录
            new CodeMsgModel(){ code = (int)CodeEnum.用户未授权, msg = "用户未授权" }
        };

        private Singleton_CodeMsg() { }

        public static List<CodeMsgModel> GetInstance()
        {
            return instance;
        }
    }
}
