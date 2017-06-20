using Model_9H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL_9H
{
    public interface IConfigDAL
    {
        ConfigModel GetModel(string key);
        bool Update(string key, string value, DateTime updateTime);
    }
}
