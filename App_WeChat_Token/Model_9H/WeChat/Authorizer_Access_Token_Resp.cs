﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_9H
{
    public class Authorizer_Access_Token_Resp
    {
        public string Authorizer_Access_Token { get; set; }
        public int Expires_In { get; set; }
        public string Authorizer_Refresh_Token { get; set; }
    }
}