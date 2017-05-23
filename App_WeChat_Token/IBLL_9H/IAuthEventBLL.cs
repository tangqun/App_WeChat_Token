using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IBLL_9H
{
    public interface IAuthEventBLL
    {
        string Receive(Stream requestStream);
    }
}
