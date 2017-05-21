using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_9H
{
    public class AuthInfoModel
    {
        public int Id { get; set; }
        public string Authorizer_Appid { get; set; }
        public string Authorizer_Access_Token_Old { get; set; }
        public string Authorizer_Access_Token { get; set; }
        public int Expires_In { get; set; }
        public string Authorizer_Refresh_Token { get; set; }
        public DateTime Auth_Time { get; set; }
        public DateTime Refresh_Time { get; set; }
        public DateTime Create_Time { get; set; }
        public DateTime Update_Time { get; set; }
    }
}
