using Model_9H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL_9H
{
    public interface ICodeMsgDAL
    {
        string GetByCode(int code);
    }
}
