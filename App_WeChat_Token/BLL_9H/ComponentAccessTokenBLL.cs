using DAL_9H;
using Helper_9H;
using IBLL_9H;
using IDAL_9H;
using Model_9H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_9H
{
    public class ComponentAccessTokenBLL : IComponentAccessTokenBLL
    {
        private IConfigDAL configDAL = new ConfigDAL();
        private ICodeMsgDAL codeMsgDAL = new CodeMsgDAL();

        public RESTfulModel Get()
        {
            try
            {
                ConfigModel configModel = configDAL.GetModel("component_access_token");
                return new RESTfulModel() { Code = (int)CodeEnum.成功, Msg = string.Format(codeMsgDAL.GetByCode((int)CodeEnum.成功), "成功"), Data = configModel.Value };
            }
            catch (Exception ex)
            {
                LogHelper.Error("唐群", ex);
                return new RESTfulModel() { Code = (int)CodeEnum.系统异常, Msg = codeMsgDAL.GetByCode((int)CodeEnum.系统异常) };
            }
        }
    }
}
