using BLL_9H;
using Helper_9H;
using IBLL_9H;
using Model_9H;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshToken_9H
{
    class Program
    {
        private static IAccessTokenBLL accessTokenBLL = new AccessTokenBLL();

        static void Main(string[] args)
        {
            accessTokenBLL.RefreshForAuthorized();
        }
    }
}
